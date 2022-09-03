using AutoDependencies.Generator.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Extensions;
internal static class AttributeExtensions
{
    public static bool HasAttribute(
        this MemberDeclarationSyntax syntax, 
        string attributeName, 
        SemanticModel semanticModel)
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
                    .GetTypeInfo(attributeSyntax)
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
        return !attributeName.StartsWith(Constants.GeneratorConstants.AttributesNamespace) 
            ? $"{Constants.GeneratorConstants.AttributesNamespace}.{attributeName}" 
            : attributeName;
    }
}
