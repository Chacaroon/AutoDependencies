using AutoDependencies.Generator.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Extensions;
internal static class AttributeExtensions
{
    public static bool HasAttribute(
        this MemberDeclarationSyntax syntax, 
        string attributeName, 
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        foreach (var attributeList in syntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeList.Attributes)
            {
                var name = attributeSyntax.Name.GetText().ToString();

                if (!attributeName.StartsWith(name))
                {
                    continue;
                }

                var attributeFullName = semanticModel
                    .GetTypeInfo(attributeSyntax, cancellationToken)
                    .Type!
                    .ToDisplayString();

                if (attributeFullName == attributeName.ToAttributeFullName())
                {
                    return true;
                } 
            }
        }

        return false;
    }

    public static string ToAttributeFullName(this string attributeName)
    {
        return !attributeName.StartsWith(GeneratorConstants.PredefinedNamespaces.AttributesNamespace) 
            ? $"{GeneratorConstants.PredefinedNamespaces.AttributesNamespace}.{attributeName}" 
            : attributeName;
    }
}
