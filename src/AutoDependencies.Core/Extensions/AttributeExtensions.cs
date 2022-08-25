using System;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Factories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core.Extensions;
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
                    .GetSymbolInfo(attributeSyntax)
                    .Symbol!
                    .ContainingType
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
        return !attributeName.StartsWith(CoreConstants.AttributesNamespace) 
            ? $"{CoreConstants.AttributesNamespace}.{attributeName}" 
            : attributeName;
    }
}
