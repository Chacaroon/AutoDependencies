using System.Collections.Immutable;
using System.Text;
using AutoDependencies.Generator.Collectors;
using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Extensions;
using AutoDependencies.Generator.Models;
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
            (node, _) => PreliminaryCollector.IsCandidateForGeneration(node),
            (ctx, cancellationToken) =>
            {
                var node = (ClassDeclarationSyntax)ctx.Node;
                
                return PreliminaryCollector.IsApplicableForSourceGeneration(node, ctx.SemanticModel, cancellationToken)
                    ? node
                    : null;
            })
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

        var distinctClassDeclarations = classDeclarations.Distinct().ToArray();
        var classesToGenerate = GetInfoForGenerate(compilation, distinctClassDeclarations, context.CancellationToken);

        var serviceGenerator = new ServiceSyntaxFactory();

        foreach (var serviceInfo in classesToGenerate)
        {
            var generatedService = serviceGenerator.GenerateService(serviceInfo).GetText(Encoding.UTF8);
            var fileName = serviceInfo.ServiceInfo.Name.ValueText.ToGeneratedFileName();
            
            context.AddSource(fileName, generatedService);
        }
    }

    private static List<ServiceToGenerateInfo> GetInfoForGenerate(
        Compilation compilation, 
        ClassDeclarationSyntax[] classDeclarations, 
        CancellationToken cancellationToken)
    {
        var servicesToGenerate = new List<ServiceToGenerateInfo>(classDeclarations.Length);

        foreach (var classDeclarationSyntax in classDeclarations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            var serviceAnalyzer = new ServiceCollector(semanticModel);
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