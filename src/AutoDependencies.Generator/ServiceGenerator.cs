using System.Collections.Immutable;
using System.Text;
using AutoDependencies.Core;
using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using AutoDependencies.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator;

[Generator]
public class ServiceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateDefaultAttributes);

        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => ServiceAnalyzer.IsCandidateForGeneration(node),
            (ctx, _) =>
                ServiceAnalyzer.IsApplicableForSourceGeneration((ClassDeclarationSyntax)ctx.Node, ctx.SemanticModel)
                ? (ClassDeclarationSyntax)ctx.Node
                : null)
            .Where(x => x is not null)!;

        var compilationsAndClassDeclarations = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationsAndClassDeclarations,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classDeclarations, SourceProductionContext context)
    {
        if (classDeclarations.IsDefaultOrEmpty)
        {
            return;
        }

        var distinctClassDeclarations = classDeclarations.Distinct();
        var classesToGenerate = GetInfoForGenerate(compilation, distinctClassDeclarations, context.CancellationToken);

        var serviceGenerator = new Core.ServiceGenerator();

        foreach (var serviceInfo in classesToGenerate)
        {
            var generatedService = serviceGenerator.GenerateService(serviceInfo).GetText(Encoding.UTF8);
            var fileName = serviceInfo.ServiceInfo.Name.ValueText.ToGeneratedFileName();
            
            context.AddSource(fileName, generatedService);
        }
    }

    private static List<ServiceToGenerateInfo> GetInfoForGenerate(
        Compilation compilation, 
        IEnumerable<ClassDeclarationSyntax> classDeclarations, 
        CancellationToken cancellationToken)
    {
        var servicesToGenerate = new List<ServiceToGenerateInfo>(classDeclarations.Count());

        foreach (var classDeclarationSyntax in classDeclarations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            var serviceAnalyzer = new ServiceAnalyzer(semanticModel);
            var serviceToGenerateInfo = serviceAnalyzer.GetServiceToGenerateInfo(classDeclarationSyntax);

            servicesToGenerate.Add(serviceToGenerateInfo);
        }

        return servicesToGenerate;
    }

    private void GenerateDefaultAttributes(IncrementalGeneratorPostInitializationContext context)
    {
        var defaultAttributes = DefaultAttributes.GetOrCreateDefaultAttributes(context.CancellationToken);

        foreach (var attributeData in defaultAttributes)
        {
            context.AddSource(attributeData.Key.ToGeneratedFileName(), attributeData.Value.GetText(Encoding.UTF8));
        }
    }
}