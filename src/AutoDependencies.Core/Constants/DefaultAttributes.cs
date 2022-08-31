using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Core.Factories;
using Microsoft.CodeAnalysis;

namespace AutoDependencies.Core.Constants;
public static class DefaultAttributes
{
    private static readonly (string Name, AttributeTargets[] Targets)[] DefaultAttributeNames = {
        (CoreConstants.GeneratedAttributeName, new[] { AttributeTargets.Interface, AttributeTargets.Class }),
        (CoreConstants.InjectAttributeName, new[] { AttributeTargets.Property, AttributeTargets.Field}),
        (CoreConstants.ServiceAttributeName, new[] { AttributeTargets.Class })

    };
    private static readonly ConcurrentDictionary<string, SyntaxNode> AttributeDeclarations = new();

    public static IReadOnlyDictionary<string, SyntaxNode> GetOrCreateDefaultAttributes(CancellationToken cancellationToken = default)
    {
        if (AttributeDeclarations.Count == DefaultAttributeNames.Length)
        {
            return AttributeDeclarations;
        }

        foreach (var (name, targets) in DefaultAttributeNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (AttributeDeclarations.ContainsKey(name))
            {
                continue;
            }

            var attributeDeclarationSyntax = AttributeSyntaxFactory.GetOrCreateAttributeDeclarationSyntax(
                name,
                targets,
                CoreConstants.AttributesNamespace);

            AttributeDeclarations[name] = attributeDeclarationSyntax;
        }

        return AttributeDeclarations;
    }
}
