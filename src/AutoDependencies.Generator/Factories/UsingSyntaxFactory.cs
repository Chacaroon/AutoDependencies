using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Factories;
internal class UsingSyntaxFactory
{
    public static SyntaxList<UsingDirectiveSyntax> CreateUsingDirectiveList(string[] namespaces)
    {
        var usingDirectives = namespaces
            .Select(x => SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(x)))
            .ToArray();

        return SyntaxFactory.List(usingDirectives);
    }
}
