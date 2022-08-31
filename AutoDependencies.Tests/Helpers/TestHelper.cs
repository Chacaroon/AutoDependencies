using System.Collections.Immutable;
using System.Diagnostics;
using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Factories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoDependencies.Tests.Helpers;
internal static class TestHelper
{
    public static (string GeneratedOutput, ImmutableArray<Diagnostic> Diagnostics) GetGeneratedOutput<T>(string source, string[]? additionalSources = null)
        where T : IIncrementalGenerator, new()
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
            .Select(_ => MetadataReference.CreateFromFile(_.Location))
            .Concat(new[]
            {
                MetadataReference.CreateFromFile(typeof(T).Assembly.Location),
            });

        var syntaxTrees = (additionalSources?.Select(x => CSharpSyntaxTree.ParseText(x)) ?? Array.Empty<SyntaxTree>())
            .Concat(new[] { CSharpSyntaxTree.ParseText(source) });

        var compilation = CSharpCompilation.Create("Tests", syntaxTrees, references);
        var originalTreesCount = compilation.SyntaxTrees.Length 
                                 + DefaultAttributes.GetOrCreateDefaultAttributes().Count;

        var generator = new T();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var generatedSyntaxTrees = outputCompilation.SyntaxTrees.ToArray();
        var generatedOutput = originalTreesCount != generatedSyntaxTrees.Length
            ? generatedSyntaxTrees[^1].ToString()
            : string.Empty;

        return (GeneratedOutput: generatedOutput, Diagnostics: diagnostics);
    }
}
