using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace AutoDependencies.Core;
public static class DefaultAttributes
{
    private static IEnumerable<(string, SyntaxNode)>? _attributes;

    public static IEnumerable<(string, SyntaxNode)> GetOrCreateDefaultAttributes(string namespaceName)
    {
        return _attributes ??= new (string, SyntaxNode)[]
        {
            (
                CoreConstants.GeneratedAttributeName,
                AttributesManager.GetOrCreateAttributeDeclaration(
                    CoreConstants.GeneratedAttributeName,
                    new[] { AttributeTargets.Interface, AttributeTargets.Class },
                    namespaceName)
            ),
            (
                CoreConstants.ServiceAttributeName,
                AttributesManager.GetOrCreateAttributeDeclaration(
                    CoreConstants.ServiceAttributeName,
                    new[] { AttributeTargets.Class },
                    namespaceName)
            ),
            (
                CoreConstants.InjectAttributeName,
                AttributesManager.GetOrCreateAttributeDeclaration(
                    CoreConstants.InjectAttributeName,
                    new[] { AttributeTargets.Field, AttributeTargets.Property },
                    namespaceName)
            )
        };
    }
}
