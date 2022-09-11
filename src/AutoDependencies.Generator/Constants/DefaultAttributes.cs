using System.Collections.Concurrent;
using AutoDependencies.Generator.SyntaxFactories;
using Microsoft.CodeAnalysis;

namespace AutoDependencies.Generator.Constants;
public static class DefaultAttributes
{
    private static readonly (string Name, AttributeTargets[] Targets)[] DefaultAttributeNames = {
        (AttributeNames.GeneratedAttribute, new[] { AttributeTargets.Interface, AttributeTargets.Class }),
        (AttributeNames.InjectAttribute, new[] { AttributeTargets.Property, AttributeTargets.Field}),
        (AttributeNames.ServiceAttribute, new[] { AttributeTargets.Class })

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
                PredefinedNamespaces.AttributesNamespace);

            AttributeDeclarations[name] = attributeDeclarationSyntax;
        }

        return AttributeDeclarations;
    }
}
