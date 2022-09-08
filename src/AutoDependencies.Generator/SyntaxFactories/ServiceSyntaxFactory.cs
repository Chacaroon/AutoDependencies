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
        var (serviceInfo, interfaceMembersInfo, constructorMembersInfo, nullableEnabled) = serviceToGenerateInfo;

        var interfaceDeclaration = InterfaceSyntaxFactory.CreateInterfaceDeclarationSyntax(serviceInfo.InterfaceName, interfaceMembersInfo);
        var constructorDeclarationSyntax = ConstructorSyntaxFactory.CreateConstructor(serviceInfo, constructorMembersInfo);

        var classDeclaration = ClassSyntaxFactory.GeneratePartialClassService(serviceInfo, constructorDeclarationSyntax);

        var namespaceDeclaration = NamespaceSyntaxFactory.CreateNamespace(
            serviceInfo.Namespace,
            new MemberDeclarationSyntax[] { classDeclaration, interfaceDeclaration });

        if (nullableEnabled)
        {
            namespaceDeclaration = namespaceDeclaration.WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));
        }

        var root = CompilationUnit()
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                namespaceDeclaration
            }))
            .WithUsings(UsingSyntaxFactory.CreateUsingDirectiveList(new[]
            {
                GeneratorConstants.PredefinedNamespaces.AttributesNamespace
            }));

        return root.NormalizeWhitespace();
    }
}
