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
    var serviceManager = new ServiceManager(semanticModel!);
    var visitor = new ServiceVisitor(serviceManager);

    visitor.Visit(await document.GetSyntaxRootAsync());
    
    var services = visitor.ServiceNodes.Select(x => serviceManager.GenerateService(x));
    
    generatedServices.AddRange(services);
}

workspaceManager.AddDocuments(ConsoleConstants.AutoDependenciesServicesProjectName, generatedServices);

