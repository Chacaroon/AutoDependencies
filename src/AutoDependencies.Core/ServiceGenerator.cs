using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using AutoDependencies.Core.Factories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core;
public class ServiceGenerator
{
    private readonly SemanticModel _semanticModel;

    public ServiceGenerator(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public static bool IsCandidateForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    public bool IsApplicableForSourceGeneration(ClassDeclarationSyntax node)
    {
        if (!node.Modifiers.Any(SyntaxKind.PartialKeyword) || node.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return false;
        }

        if (!node.AttributeLists.Any())
        {
            return false;
        }

        return node.HasAttribute(CoreConstants.ServiceAttributeName, _semanticModel);
    }

    public (string FileName, SyntaxNode Node) GenerateService(
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        var interfaceDeclaration = InterfaceSyntaxFactory.CreateInterfaceDeclarationSyntax(classDeclarationSyntax);
        var interfaceIdentifier = SyntaxFactory.IdentifierName(interfaceDeclaration.Identifier.Text);

        var classDeclaration = ClassSyntaxFactory.GeneratePartialClassWithInterface(
            classDeclarationSyntax, 
            interfaceIdentifier,
            _semanticModel);

        var namespaceDeclaration = CreateNamespaceDeclarationSyntax(
            classDeclarationSyntax,
            new MemberDeclarationSyntax[] { classDeclaration, interfaceDeclaration });

        var root = SyntaxFactory.CompilationUnit()
            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                namespaceDeclaration
            }))
            .WithUsings(CommonMembersSyntaxFactory.CreateUsingDirectiveList(new[]
            {
                CoreConstants.AttributesNamespace
            }));

        return (classDeclarationSyntax.Identifier.Text, root);
    }

    private NamespaceDeclarationSyntax CreateNamespaceDeclarationSyntax(
        ClassDeclarationSyntax sourceClassDeclarationSyntax,
        MemberDeclarationSyntax[] members)
    {
        var namespaceName = _semanticModel
            .GetDeclaredSymbol(sourceClassDeclarationSyntax)!
            .ContainingNamespace
            .ToDisplayString();

        var namespaceDeclarationSyntax = CommonMembersSyntaxFactory.CreateNamespace(namespaceName, members);

        return namespaceDeclarationSyntax;
    }
}
