using System.Collections.Immutable;
using System.Reflection;
using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoDependencies.Tests.Helpers;
internal static class TestHelper
{
    public static GeneratedOutput GetGeneratedOutput<T>(
        string source,
        string[]? additionalSources = null,
        NullableContextOptions nullableContextOptions = NullableContextOptions.Disable)
        where T : IIncrementalGenerator, new()
    {
        var (
            compilation, 
            diagnostics,
            originalSyntaxTreesCount
            ) = RunGenerator<T>(source, additionalSources, nullableContextOptions);

        var generatedSyntaxTrees = compilation.SyntaxTrees.ToArray();
        var generatedOutput = originalSyntaxTreesCount != generatedSyntaxTrees.Length
            ? generatedSyntaxTrees[^1].ToString()
            : string.Empty;

        var serviceCollectionExtensions = generatedSyntaxTrees
            .FirstOrDefault(x => Path.GetFileName(x.FilePath) == "ServiceCollectionExtensions".ToGeneratedFileName())?
            .GetText()
            .ToString()
            ?? string.Empty;

        return new(
            Output: generatedOutput, 
            Diagnostics: diagnostics.ToArray(), 
            ServiceCollectionExtensions: serviceCollectionExtensions);
    }

    private static GenerationResult RunGenerator<T>(
        string source,
        string[]? additionalSources = null,
        NullableContextOptions nullableContextOptions = NullableContextOptions.Disable)
        where T : IIncrementalGenerator, new()
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
            .Select(_ => MetadataReference.CreateFromFile(_.Location))
            .Concat(new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(T).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
            });

        var syntaxTrees = (additionalSources?.Select(x => CSharpSyntaxTree.ParseText(x)) ?? Array.Empty<SyntaxTree>())
            .Concat(new[] { CSharpSyntaxTree.ParseText(source) });

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            nullableContextOptions: nullableContextOptions);

        var compilation = CSharpCompilation.Create("Tests", syntaxTrees, references)
            .WithOptions(compilationOptions);

        var originalTreesCount = compilation.SyntaxTrees.Length
                                 + DefaultAttributes.GetOrCreateDefaultAttributes().Count;

        var generator = new T();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        return new(outputCompilation, diagnostics, originalTreesCount);
    }

    private record GenerationResult(
        Compilation Compilation,
        ImmutableArray<Diagnostic> Diagnostics,
        int OriginalSyntaxTreesCount);

    public record GeneratedOutput(
        string? Output, 
        Diagnostic[] Diagnostics, 
        string? ServiceCollectionExtensions);
}
