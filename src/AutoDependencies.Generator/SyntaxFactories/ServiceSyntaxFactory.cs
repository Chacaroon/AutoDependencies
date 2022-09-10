using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
public static class ServiceSyntaxFactory
{
    public static SyntaxNode GenerateServiceSyntax(ServiceToGenerateInfo serviceToGenerateInfo)
    {
        var (serviceInfo, interfaceInfo, constructorMembersInfo, nullableEnabled) = serviceToGenerateInfo;

        var interfaceDeclaration = InterfaceSyntaxFactory.CreateInterfaceDeclarationSyntax(serviceInfo.InterfaceName, interfaceInfo.InterfaceMembers);
        var constructorDeclarationSyntax = ConstructorSyntaxFactory.CreateConstructorSyntax(serviceInfo, constructorMembersInfo);

        var classDeclaration = ClassSyntaxFactory.GeneratePartialClassServiceSyntax(serviceInfo, constructorDeclarationSyntax);
        
        var serviceNamespaceDeclaration = NamespaceDeclaration(IdentifierName(serviceInfo.NamespaceName))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDeclaration }));

        var interfaceNamespaceDeclaration = NamespaceDeclaration(IdentifierName(interfaceInfo.NamespaceName))
            .WithMembers(List(new MemberDeclarationSyntax[] { interfaceDeclaration }));

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
            .WithUsings(UsingSyntaxFactory.CreateUsingDirectiveListSyntax(new[]
            {
                GeneratorConstants.PredefinedNamespaces.AttributesNamespace,
                interfaceInfo.NamespaceName
            }));

        return root.NormalizeWhitespace();
    }
}
