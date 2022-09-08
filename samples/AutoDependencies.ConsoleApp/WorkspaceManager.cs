using System.Collections.Immutable;
using System.Reflection;
using AutoDependencies.Generator.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace AutoDependencies.ConsoleApp;
internal class WorkspaceManager
{
    private static readonly string SolutionPath = Path.Combine(
        Assembly.GetEntryAssembly()!.Location,
        "../../../../../../AutoDependencies.sln");

    private readonly MSBuildWorkspace _workspace;

    public WorkspaceManager()
    {
        _workspace = WorkspaceFactory.GetOrCreateWorkspace();
    }

    public Solution GetSolution() => _workspace.CurrentSolution;

    public Project GetProject(string projectName) => GetSolution().Projects.First(x => x.Name == projectName);

    public async Task PrepareWorkspaceAsync(string projectName)
    {
        await _workspace.OpenSolutionAsync(SolutionPath);

        Console.WriteLine("Remove generated files");
        RemoveDocuments(ConsoleConstants.AutoDependenciesServicesProjectName, x => x.Name.EndsWith(GeneratorConstants.GeneratedDocumentExtension));
    }

    public void AddDocuments(string projectName, IReadOnlyDictionary<string, SyntaxNode> nodes)
    {
        var project = GetProject(projectName);

        var solution = nodes.Aggregate(
            GetSolution(),
            (solution, x) => AddDocument(solution, project, x.Key, x.Value));

        var result = _workspace.TryApplyChanges(solution);

        Console.WriteLine($"Add documents: {result}");
        Console.WriteLine($"Added documents: {Environment.NewLine}{string.Join(Environment.NewLine, nodes.Select(x => x.Key))}");
        Console.WriteLine();
    }

    public void RemoveDocuments(string projectName, Func<Document, bool> predicate)
    {
        var solution = GetSolution();
        var project = GetProject(projectName);

        var documentsToDelete = project.Documents
            .Where(predicate)
            .ToArray();

        var documentNamesToDelete = documentsToDelete
            .Select(x => x.Name.Replace(GeneratorConstants.GeneratedDocumentExtension, string.Empty))
            .ToArray();

        var documentIdsToDelete = documentsToDelete
            .Select(x => x.Id)
            .ToImmutableArray();

        var newSolution = solution.RemoveDocuments(documentIdsToDelete);
        var result = _workspace.TryApplyChanges(newSolution);

        Console.WriteLine($"Remove documents: {result}");
        Console.WriteLine($"Removed documents: {Environment.NewLine}{string.Join(Environment.NewLine, documentNamesToDelete)}");
        Console.WriteLine();
    }

    private Solution AddDocument(Solution solution, Project project, string fileName, SyntaxNode node)
    {
        if (!fileName.EndsWith(GeneratorConstants.GeneratedDocumentExtension))
        {
            fileName = $"{fileName.Replace(".cs", string.Empty)}{GeneratorConstants.GeneratedDocumentExtension}";
        }

        var newSolution = solution.AddDocument(
            DocumentId.CreateNewId(project.Id),
            $"Generated/{fileName}",
            node.NormalizeWhitespace(),
            isGenerated: true);

        return newSolution;
    }
}
