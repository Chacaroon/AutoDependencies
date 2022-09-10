using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
public static class ServiceSyntaxFactory
{
    public static SyntaxNode GenerateService(ServiceToGenerateInfo serviceToGenerateInfo)
    {
        var (serviceInfo, interfaceInfo, constructorMembersInfo, nullableEnabled) = serviceToGenerateInfo;

        var interfaceDeclaration = InterfaceSyntaxFactory.CreateInterfaceDeclarationSyntax(serviceInfo.InterfaceName, interfaceInfo.InterfaceMembers);
        var constructorDeclarationSyntax = ConstructorSyntaxFactory.CreateConstructor(serviceInfo, constructorMembersInfo);

        var classDeclaration = ClassSyntaxFactory.GeneratePartialClassService(serviceInfo, constructorDeclarationSyntax);

        var serviceNamespaceDeclaration = NamespaceSyntaxFactory.CreateNamespace(
            serviceInfo.Namespace,
            new MemberDeclarationSyntax[] { classDeclaration });

        var interfaceNamespaceDeclaration = NamespaceSyntaxFactory.CreateNamespace(
            interfaceInfo.NamespaceName,
            new[] { interfaceDeclaration });

        if (nullableEnabled)
        {
            serviceNamespaceDeclaration = serviceNamespaceDeclaration.WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));
        }

        var root = CompilationUnit()
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                serviceNamespaceDeclaration,
                interfaceNamespaceDeclaration
            }))
            .WithUsings(UsingSyntaxFactory.CreateUsingDirectiveList(new[]
            {
                GeneratorConstants.PredefinedNamespaces.AttributesNamespace,
                interfaceInfo.NamespaceName
            }));

        return root.NormalizeWhitespace();
    }
}
