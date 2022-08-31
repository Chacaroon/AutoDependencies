using AutoDependencies.ConsoleApp;
using AutoDependencies.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var workspaceManager = new WorkspaceManager();
await workspaceManager.PrepareWorkspaceAsync(ConsoleConstants.AutoDependenciesServicesProjectName);

var project = workspaceManager.GetProject(ConsoleConstants.AutoDependenciesServicesProjectName);

var compilation = (await project.GetCompilationAsync())!;

var generator = new AutoDependencies.Generator.ServiceGenerator();
GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

Console.WriteLine(string.Join(Environment.NewLine, diagnostics.Select(x => x.GetMessage())));

var generatedFiles = updatedCompilation.SyntaxTrees
    .Select(x =>
    {
        var classDeclarationSyntax = x.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

        if (classDeclarationSyntax == null)
        {
            return (null, null!);
        }

        var isGeneratedClass = classDeclarationSyntax.AttributeLists
            .Any(x => x.Attributes.Any(x => x.Name.ToString() == "Generated"));

        if (!isGeneratedClass)
        {
            return (null, null!);
        }

        var className = $"{classDeclarationSyntax.Identifier}{CoreConstants.GeneratedDocumentExtension}";
        return (Name: (string?)className, Node: x.GetRoot());
    })
    .Where(x => x.Name != null)
    .ToDictionary(x => x.Name!, x => x.Node);

workspaceManager.AddDocuments(
    ConsoleConstants.AutoDependenciesServicesProjectName,
    generatedFiles);
