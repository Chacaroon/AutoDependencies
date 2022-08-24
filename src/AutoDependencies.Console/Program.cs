using AutoDependencies.ConsoleApp;
using AutoDependencies.Core;
using Microsoft.CodeAnalysis;

var workspaceManager = new WorkspaceManager();
await workspaceManager.PrepareWorkspaceAsync(ConsoleConstants.AutoDependenciesServicesProjectName);

var serviceManager = new ServiceManager();

var project = workspaceManager.GetProject(ConsoleConstants.AutoDependenciesServicesProjectName);

var generatedServices = new List<(string, SyntaxNode)>();

foreach (var document in project.Documents)
{
    var visitor = new ServiceVisitor();
    visitor.Visit(await document.GetSyntaxRootAsync());

    var semanticModel = await document.GetSemanticModelAsync();
    var services = visitor.ServiceNodes.Select(x => serviceManager.GenerateService(x, semanticModel!));
    generatedServices.AddRange(services);
}

workspaceManager.AddDocuments(ConsoleConstants.AutoDependenciesServicesProjectName, generatedServices);

