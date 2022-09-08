using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Extensions;

internal static class TypeSyntaxExtensions
{
    public static TypeSyntax ToFullNameTypeSyntax(this TypeSyntax typeSyntax, SemanticModel semanticModel)
    {
        var typeSymbol = semanticModel.GetTypeInfo(typeSyntax).Type;

        if (typeSymbol == null)
        {
            return IdentifierName(typeSyntax.ToFullString().Trim());
        }

        if (typeSyntax is NullableTypeSyntax)
        {
            typeSymbol = typeSymbol.WithNullableAnnotation(NullableAnnotation.Annotated);
        }

        var typeFullName = typeSymbol.ToDisplayString();

        return IdentifierName(typeFullName);
    } 
}