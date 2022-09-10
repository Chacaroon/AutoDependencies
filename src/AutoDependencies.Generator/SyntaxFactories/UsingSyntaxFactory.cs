using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
internal class UsingSyntaxFactory
{
    public static SyntaxList<UsingDirectiveSyntax> CreateUsingDirectiveListSyntax(string[] namespaces)
    {
        var usingDirectives = namespaces
            .Select(x => UsingDirective(IdentifierName(x)))
            .ToArray();

        return List(usingDirectives);
    }
}
