using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDependencies.Core.Factories;
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
