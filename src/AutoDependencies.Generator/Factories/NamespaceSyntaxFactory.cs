using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Factories;
internal static class NamespaceSyntaxFactory
{
    public static NamespaceDeclarationSyntax CreateNamespace(string namespaceName, IEnumerable<MemberDeclarationSyntax>? members = null)
    {
        var namespaceIdentifier = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(namespaceName));

        var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(namespaceIdentifier)
            .WithMembers(SyntaxFactory.List(members ?? Array.Empty<MemberDeclarationSyntax>()));

        return namespaceDeclaration;
    }
}
