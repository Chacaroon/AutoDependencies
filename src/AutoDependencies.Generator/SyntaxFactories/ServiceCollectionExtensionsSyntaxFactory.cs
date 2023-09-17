using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
public static class ServiceCollectionExtensionsSyntaxFactory
{
    private static readonly IdentifierNameSyntax ServicesIdentifierSyntax = IdentifierName("services");
    private static readonly IdentifierNameSyntax LifetimeIdentifierSyntax = IdentifierName("lifetime");
    private static readonly IdentifierNameSyntax ServiceCollectionIdentifierSyntax =
        IdentifierName("IServiceCollection");
    private static readonly IdentifierNameSyntax ServiceLifetimeIdentifierNameSyntax = 
        IdentifierName("ServiceLifetime");

    public static CompilationUnitSyntax? CreateServiceCollectionExtensionsSyntax(
        string assemblyName, 
        ServiceToGenerateInfo[] serviceToGenerateInfos)
    {
        if (serviceToGenerateInfos.Length == 0)
        {
            return null;
        }

        var normalizedAssemblyName = assemblyName.Replace(".", string.Empty);

        var extensionMethodDeclaration = CreateExtensionMethodDeclaration(serviceToGenerateInfos, normalizedAssemblyName);

        var classDeclaration = ClassDeclaration($"{normalizedAssemblyName}{PredefinedClassNames.ServiceCollectionExtensions}")
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithMembers(List(new MemberDeclarationSyntax[] { extensionMethodDeclaration }))
            .WithAttributeLists(List(new[]
            {
                AttributeSyntaxFactory
                    .GetOrCreateAttributeListSyntax(AttributeNames.GeneratedAttribute)
            }));

        var namespaceName =
            $"{assemblyName}.{PredefinedNamespaces.GeneratedExtensionsNamespacePart}";
        var namespaceSyntax = NamespaceDeclaration(IdentifierName(namespaceName))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDeclaration }));

        var compilationUnitSyntax = CompilationUnit()
            .WithUsings(UsingSyntaxFactory.CreateUsingDirectiveListSyntax(
                serviceToGenerateInfos.Select(x => x.ServiceInfo.NamespaceName).Distinct().Concat(new[]
                {
                    PredefinedNamespaces.AttributesNamespace,
                    PredefinedNamespaces.DependencyInjectionNamespace,
                    serviceToGenerateInfos[0].InterfaceInfo.NamespaceName
                }).ToArray()))
            .WithMembers(List(new MemberDeclarationSyntax[] { namespaceSyntax }))
            .NormalizeWhitespace();

        return compilationUnitSyntax;
    }

    private static MethodDeclarationSyntax CreateExtensionMethodDeclaration(
        ServiceToGenerateInfo[] serviceToGenerateInfos, 
        string normalizedAssemblyName)
    {
        var serviceParameter = Parameter(ServicesIdentifierSyntax.Identifier)
            .WithType(ServiceCollectionIdentifierSyntax)
            .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)));

        var lifetimeScopeParameter = Parameter(LifetimeIdentifierSyntax.Identifier)
            .WithType(ServiceLifetimeIdentifierNameSyntax);

        var registrationExpressions = CreateRegistrationExpressions(
            serviceToGenerateInfos, 
            ServicesIdentifierSyntax, 
            LifetimeIdentifierSyntax);
        
        var methodBody = Block(registrationExpressions)
            .AddStatements(ReturnStatement(ServicesIdentifierSyntax));

        return MethodDeclaration(ServiceCollectionIdentifierSyntax, $"RegisterServicesForm{normalizedAssemblyName}")
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(ParameterList(SeparatedList(new[]
            {
                serviceParameter,
                lifetimeScopeParameter
            })))
            .WithBody(methodBody);
    }

    private static StatementSyntax[] CreateRegistrationExpressions(
        ServiceToGenerateInfo[] services,
        IdentifierNameSyntax servicesIdentifier,
        IdentifierNameSyntax lifetimeIdentifier)
    {
        var result = new StatementSyntax[services.Length];

        for (var i = 0; i < services.Length; i++)
        {
            result[i] = ServiceRegistrationSyntaxFactory.CreateServiceRegistrationCallSyntax(
                services[i].ServiceInfo,
                services[i].InterfaceInfo,
                servicesIdentifier,
                lifetimeIdentifier);
        }

        return result;
    }
}
