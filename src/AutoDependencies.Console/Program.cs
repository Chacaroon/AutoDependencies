using AutoDependencies.ConsoleApp;
using AutoDependencies.Core;
using Microsoft.CodeAnalysis;

var workspaceManager = new WorkspaceManager();
await workspaceManager.PrepareWorkspaceAsync(ConsoleConstants.AutoDependenciesServicesProjectName);

var project = workspaceManager.GetProject(ConsoleConstants.AutoDependenciesServicesProjectName);

var generatedServices = new List<(string, SyntaxNode)>();

foreach (var document in project.Documents)
{
    var semanticModel = await document.GetSemanticModelAsync();
    var serviceAnalyzer = new ServiceAnalyzer(semanticModel!);
    var serviceManager = new ServiceGenerator();
    var visitor = new ServiceVisitor(serviceAnalyzer);

    visitor.Visit(await document.GetSyntaxRootAsync());
    
    var services = visitor.ServiceNodes.Select(classDeclarationSyntax =>
    {
        var serviceInfo = serviceAnalyzer.GetServiceInfo(classDeclarationSyntax);
        var constructorMembersInfo = serviceAnalyzer.GetConstructorMembersInfo(classDeclarationSyntax);
        var interfaceMembersInfo = serviceAnalyzer.GetInterfaceMembersInfo(classDeclarationSyntax);

        return (serviceInfo.Name.ValueText, serviceManager.GenerateService(serviceInfo, constructorMembersInfo, interfaceMembersInfo));
    });
    
    generatedServices.AddRange(services);
}

workspaceManager.AddDocuments(ConsoleConstants.AutoDependenciesServicesProjectName, generatedServices);
