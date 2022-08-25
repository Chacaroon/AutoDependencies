using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using AutoDependencies.Core.Factories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core;
public class ServiceGenerator
{
    private readonly SemanticModel _documentSemanticModel;

    public ServiceGenerator(
        SemanticModel documentSemanticModel)
    {
        _documentSemanticModel = documentSemanticModel;
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

        return node.HasAttribute(CoreConstants.ServiceAttributeName, _documentSemanticModel);
    }

    public (string FileName, SyntaxNode Node) GenerateService(
        ClassDeclarationSyntax classDeclarationSyntax,
        SemanticModel semanticModel)
    {
        var (interfaceDeclaration, interfaceIdentifier) = InterfaceSyntaxFactory.CreateInterfaceForClass(classDeclarationSyntax);

        var classDeclaration = ClassSyntaxFactory.GeneratePartialClassWithInterface(
            classDeclarationSyntax, 
            interfaceIdentifier,
            semanticModel);

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
        var namespaceName = _documentSemanticModel
            .GetDeclaredSymbol(sourceClassDeclarationSyntax)!
            .ContainingNamespace
            .ToDisplayString();

        var namespaceDeclarationSyntax = CommonMembersSyntaxFactory.CreateNamespace(namespaceName, members);

        return namespaceDeclarationSyntax;
    }
}
