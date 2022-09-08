using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
internal static class NamespaceSyntaxFactory
{
    public static NamespaceDeclarationSyntax CreateNamespace(string namespaceName, IEnumerable<MemberDeclarationSyntax>? members = null)
    {
        var namespaceIdentifier = IdentifierName(Identifier(namespaceName));

        var namespaceDeclaration = NamespaceDeclaration(namespaceIdentifier)
            .WithMembers(List(members ?? Array.Empty<MemberDeclarationSyntax>()));

        return namespaceDeclaration;
    }
}
