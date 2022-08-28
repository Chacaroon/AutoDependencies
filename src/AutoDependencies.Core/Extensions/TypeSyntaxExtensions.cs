using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core.Extensions;

internal static class TypeSyntaxExtensions
{
    public static TypeSyntax ToFullNameTypeSyntax(this TypeSyntax typeSyntax, SemanticModel semanticModel)
    {
        var typeFullName = semanticModel.GetTypeInfo(typeSyntax).Type?.ToDisplayString()
            ?? typeSyntax.ToFullString().Trim();

        return SyntaxFactory.IdentifierName(typeFullName);
    } 
}