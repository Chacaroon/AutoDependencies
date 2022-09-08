using AutoDependencies.Generator.Constants;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Generator.Extensions;

namespace AutoDependencies.Generator.Collectors;

internal static class PreliminaryCollector
{
    private static readonly SyntaxKind[] ForbiddenModifiers = {
        SyntaxKind.StaticKeyword,
        SyntaxKind.AbstractKeyword
    };

    public static bool IsCandidateForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    public static bool IsApplicableForSourceGeneration(
        ClassDeclarationSyntax node, 
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        if (ForbiddenModifiers.Any(x => node.Modifiers.Any(x)))
        {
            return false;
        }

        return node.HasAttribute(GeneratorConstants.AttributeNames.ServiceAttribute, semanticModel, cancellationToken);
    }
}