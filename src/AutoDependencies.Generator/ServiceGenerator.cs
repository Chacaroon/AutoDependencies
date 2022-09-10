using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using AutoDependencies.Generator.Collectors;
using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Extensions;
using AutoDependencies.Generator.Models;
using AutoDependencies.Generator.SyntaxFactories;
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
            (node, _) => PreliminaryInfoCollector.IsCandidateForGeneration(node),
            (ctx, cancellationToken) =>
            {
                var node = (ClassDeclarationSyntax)ctx.Node;

                return PreliminaryInfoCollector.IsApplicableForSourceGeneration(node, ctx.SemanticModel, cancellationToken)
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

        var extensionsDeclaration = ServiceCollectionExtensionsSyntaxFactory.CreateServiceCollectionExtensionsSyntax(
            compilation.AssemblyName!,
            classesToGenerate);

        if (extensionsDeclaration != null)
        {
            context.AddSource("ServiceCollectionExtensions".ToGeneratedFileName(), extensionsDeclaration.GetText(Encoding.UTF8));
        }

        foreach (var serviceInfo in classesToGenerate)
        {
            var generatedService = ServiceSyntaxFactory.GenerateServiceSyntax(serviceInfo).GetText(Encoding.UTF8);
            var fileName = serviceInfo.ServiceInfo.ServiceName.ValueText.ToGeneratedFileName();

            context.AddSource(fileName, generatedService);
        }
    }

    private static ServiceToGenerateInfo[] GetInfoForGenerate(
        Compilation compilation,
        ClassDeclarationSyntax[] classDeclarations,
        CancellationToken cancellationToken)
    {
        var servicesToGenerate = new ServiceToGenerateInfo[classDeclarations.Length];

        for (var i = 0; i < classDeclarations.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(classDeclarations[i].SyntaxTree);
            var nullableEnabled = compilation.Options.NullableContextOptions == NullableContextOptions.Enable;

            var serviceToGenerateInfo = ServiceCollector.GetServiceToGenerateInfo(classDeclarations[i], semanticModel, nullableEnabled);

            servicesToGenerate[i] = serviceToGenerateInfo;
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