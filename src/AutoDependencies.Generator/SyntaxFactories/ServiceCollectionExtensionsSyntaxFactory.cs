using System;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
public static class ServiceCollectionExtensionsSyntaxFactory
{
    public static CompilationUnitSyntax? CreateServiceCollectionExtensionsSyntax(string assemblyName, ServiceToGenerateInfo[] classToGenerateInfos)
    {
        if (classToGenerateInfos.Length == 0)
        {
            return null;
        }

        var normalizedAssemblyName = assemblyName.Replace(".", string.Empty);

        var servicesIdentifier = Identifier("services");
        var lifetimeIdentifier = Identifier("lifetime");

        var serviceCollectionType = IdentifierName("IServiceCollection");

        var serviceParameter = Parameter(servicesIdentifier)
            .WithType(serviceCollectionType);
        var lifetimeScopeParameter = Parameter(lifetimeIdentifier)
            .WithType(IdentifierName("ServiceLifetime"));

        var registrationExpressions = CreateRegistrationExpressions(classToGenerateInfos, servicesIdentifier, lifetimeIdentifier);
        var returnExpression = ReturnStatement(IdentifierName(servicesIdentifier));
        var methodBody = Block(registrationExpressions).AddStatements(returnExpression);

        var extensionMethodDeclaration = MethodDeclaration(serviceCollectionType, $"RegisterServicesForm{normalizedAssemblyName}")
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(ParameterList(SeparatedList(new SyntaxNode[]
                {
                    serviceParameter.WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))),
                    lifetimeScopeParameter
                })))
                .WithBody(methodBody);

        var classDeclaration = ClassDeclaration($"{normalizedAssemblyName}ServiceCollectionExtensions")
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithMembers(List(new MemberDeclarationSyntax[] { extensionMethodDeclaration }))
            .WithAttributeLists(List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(GeneratorConstants.AttributeNames.GeneratedAttribute)
            }));

        var namespaceSyntax = NamespaceDeclaration(IdentifierName($"{assemblyName}.Extensions.Generated"))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDeclaration }));

        var compilationUnitSyntax = CompilationUnit()
            .WithUsings(UsingSyntaxFactory.CreateUsingDirectiveListSyntax(
                classToGenerateInfos.Select(x => x.ServiceInfo.NamespaceName).Distinct().Concat(new[]
                {
                    GeneratorConstants.PredefinedNamespaces.AttributesNamespace,
                    "Microsoft.Extensions.DependencyInjection",
                    classToGenerateInfos[0].InterfaceInfo.NamespaceName
                }).ToArray()))
            .WithMembers(List(new MemberDeclarationSyntax[] { namespaceSyntax }))
            .NormalizeWhitespace();

        return compilationUnitSyntax;
    }

    private static StatementSyntax[] CreateRegistrationExpressions(
        ServiceToGenerateInfo[] services,
        SyntaxToken servicesIdentifier,
        SyntaxToken lifetimeIdentifier)
    {
        var result = new StatementSyntax[services.Length];

        for (var i = 0; i < services.Length; i++)
        {
            result[i] = ServiceRegistrationSyntaxFactory.CreateServiceRegistrationCallSyntax(
                services[i].ServiceInfo,
                servicesIdentifier,
                lifetimeIdentifier);
        }

        return result;
    }
}
