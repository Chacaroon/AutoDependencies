﻿using AutoDependencies.Generator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Collectors;

internal static class PreliminaryInfoCollector
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

        return node.HasAttribute(AttributeNames.ServiceAttribute, semanticModel, cancellationToken);
    }
}