using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
public static class ServiceSyntaxFactory
{
    public static SyntaxNode GenerateServiceSyntax(ServiceToGenerateInfo serviceToGenerateInfo)
    {
        var namespaceWithInterface = CreateNamespaceWithInterface(serviceToGenerateInfo.InterfaceInfo);
        var namespaceWithService = CreateNamespaceWithService(serviceToGenerateInfo);
        
        var root = CompilationUnit()
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                namespaceWithService,
                namespaceWithInterface
            }))
            .WithUsings(UsingSyntaxFactory.CreateUsingDirectiveListSyntax(new[]
            {
                PredefinedNamespaces.AttributesNamespace,
                serviceToGenerateInfo.InterfaceInfo.NamespaceName
            }));

        return root.NormalizeWhitespace();
    }

    private static NamespaceDeclarationSyntax CreateNamespaceWithInterface(InterfaceInfo interfaceInfo)
    {
        var interfaceDeclaration = InterfaceSyntaxFactory.CreateInterfaceDeclarationSyntax(
            IdentifierName(interfaceInfo.InterfaceName), 
            interfaceInfo.InterfaceMembers);
        
        return NamespaceDeclaration(IdentifierName(interfaceInfo.NamespaceName))
            .WithMembers(List(new MemberDeclarationSyntax[] { interfaceDeclaration }));
    }

    private static NamespaceDeclarationSyntax CreateNamespaceWithService(ServiceToGenerateInfo serviceToGenerateInfo)
    {
        var (serviceInfo, interfaceInfo, constructorInfo, nullableEnabled) = serviceToGenerateInfo;

        var constructorDeclarationSyntax = ConstructorSyntaxFactory.CreateConstructorSyntax(serviceInfo, constructorInfo);
        var classDeclaration = ClassSyntaxFactory.GeneratePartialClassServiceSyntax(serviceInfo, interfaceInfo, constructorDeclarationSyntax);

        var serviceNamespaceDeclaration = NamespaceDeclaration(IdentifierName(serviceInfo.NamespaceName))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDeclaration }));

        if (nullableEnabled)
        {
            serviceNamespaceDeclaration = serviceNamespaceDeclaration.WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));
        }

        return serviceNamespaceDeclaration;
    }
}
