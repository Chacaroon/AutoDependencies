﻿using System.Collections.Immutable;
using System.Reflection;
using AutoDependencies.Generator.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoDependencies.Tests.Helpers;
internal static class TestHelper
{
    public static (string GeneratedOutput, ImmutableArray<Diagnostic> Diagnostics) GetGeneratedOutput<T>(
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

        var generatedSyntaxTrees = outputCompilation.SyntaxTrees.ToArray();
        var generatedOutput = originalTreesCount != generatedSyntaxTrees.Length
            ? generatedSyntaxTrees[^1].ToString()
            : string.Empty;

        return (GeneratedOutput: generatedOutput, Diagnostics: diagnostics);
    }
}
