using System;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Core.Factories;
using Microsoft.CodeAnalysis;

namespace AutoDependencies.Core.Constants;
public static class DefaultAttributes
{
    private static IEnumerable<(string, SyntaxNode)>? _attributes;

    public static IEnumerable<(string, SyntaxNode)> GetOrCreateDefaultAttributes()
    {
        return _attributes ??= new (string, SyntaxNode)[]
        {
            (
                CoreConstants.GeneratedAttributeName,
                AttributeSyntaxFactory.GetOrCreateAttributeDeclarationSyntax(
                    CoreConstants.GeneratedAttributeName,
                    new[] { AttributeTargets.Interface, AttributeTargets.Class },
                    CoreConstants.AttributesNamespace)
            ),
            (
                CoreConstants.ServiceAttributeName,
                AttributeSyntaxFactory.GetOrCreateAttributeDeclarationSyntax(
                    CoreConstants.ServiceAttributeName,
                    new[] { AttributeTargets.Class },
                    CoreConstants.AttributesNamespace)
            ),
            (
                CoreConstants.InjectAttributeName,
                AttributeSyntaxFactory.GetOrCreateAttributeDeclarationSyntax(
                    CoreConstants.InjectAttributeName,
                    new[] { AttributeTargets.Field, AttributeTargets.Property },
                    CoreConstants.AttributesNamespace)
            )
        };
    }
}
