using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core;
public class ServiceManager
{
    public static bool IsApplicableForSourceGeneration(ClassDeclarationSyntax node)
    {
        if (!node.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
        {
            return false;
        }

        var hasGeneratedAttribute = node.AttributeLists.Any(x =>
            x.Attributes.Any(x => x.Name == AttributesManager.GetOrCreateAttributeSyntax(CoreConstants.GeneratedAttributeName).Name));

        return !hasGeneratedAttribute;
    }

    public (string FileName, SyntaxNode Node) GenerateService(ClassDeclarationSyntax syntax, SemanticModel semanticModel)
    {
        var (interfaceDeclaration, interfaceType) = GenerateInterface(syntax);
        var interfaceList = SyntaxFactory.BaseList(
            SyntaxFactory.SeparatedList(new BaseTypeSyntax[] { SyntaxFactory.SimpleBaseType(interfaceType) }));

        var classDeclaration = GenerateClass(syntax, interfaceList);

        var namespaceName = ModelExtensions.GetDeclaredSymbol(semanticModel, syntax)!.ContainingNamespace.ToDisplayString();

        var namespaceDeclaration = SyntaxNodesFactory.CreateNamespace(
            namespaceName,
            new MemberDeclarationSyntax[] { classDeclaration, interfaceDeclaration });

        return (syntax.Identifier.Text, namespaceDeclaration);
    }

    private (InterfaceDeclarationSyntax, IdentifierNameSyntax) GenerateInterface(ClassDeclarationSyntax syntax)
    {
        var interfaceName = $"I{syntax.Identifier.Text}";
        var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName)
            .WithAttributeLists(SyntaxFactory.List(new[]
            {
                AttributesManager.GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName)
            }))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

        var interfaceType = SyntaxFactory.IdentifierName(interfaceName);

        return (interfaceDeclaration, interfaceType);
    }

    private ClassDeclarationSyntax GenerateClass(ClassDeclarationSyntax syntax, BaseListSyntax interfaces)
    {
        var classDeclaration = SyntaxFactory.ClassDeclaration(syntax.Identifier)
            .WithAttributeLists(SyntaxFactory.List(new[]
            {
                AttributesManager.GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName)
            }))
            .WithModifiers(syntax.Modifiers)
            .WithBaseList(interfaces);

        return classDeclaration;
    }
}
