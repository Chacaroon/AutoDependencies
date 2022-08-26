using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using AutoDependencies.Core.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Rename;

namespace AutoDependencies.Core;
public class ServiceAnalyzer
{
    private readonly SemanticModel _semanticModel;

    private readonly SyntaxKind[] _deniedModifiers = new[]
    {
        SyntaxKind.StaticKeyword,
        SyntaxKind.AbstractKeyword
    };

    public ServiceAnalyzer(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public static bool IsCandidateForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    public bool IsApplicableForSourceGeneration(ClassDeclarationSyntax node)
    {
        if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        if (_deniedModifiers.Any(x => node.Modifiers.Any(x)))
        {
            return false;
        }

        return node.HasAttribute(CoreConstants.ServiceAttributeName, _semanticModel);
    }

    public ServiceInfo GetServiceInfo(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var namespaceName = _semanticModel
            .GetDeclaredSymbol(classDeclarationSyntax)!
            .ContainingNamespace
            .ToDisplayString();

        return new ServiceInfo
        {
            Name = classDeclarationSyntax.Identifier,
            Modifiers = classDeclarationSyntax.Modifiers,
            Namespace = namespaceName
        };
    }

    public InterfaceMemberInfo[] GetInterfaceMembersInfo(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Where(x => x.Modifiers.Any(SyntaxKind.PublicKeyword)
                        && !x.Modifiers.Any(SyntaxKind.StaticKeyword))
            .Select(x => new InterfaceMemberInfo
            {
                Name = x.Identifier.ValueText,
                ParameterList = x.ParameterList,
                ReturnType = x.ReturnType,
            })
            .ToArray();
    }

    public ConstructorMemberInfo[] GetConstructorMembersInfo(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var memberDeclarations = classDeclarationSyntax.DescendantNodes()
            .OfType<MemberDeclarationSyntax>()
            .Where(x =>
                x.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
                || x.HasAttribute(CoreConstants.InjectAttributeName, _semanticModel))
            .ToArray();

        var fieldDeclarations = memberDeclarations
            .OfType<FieldDeclarationSyntax>()
            .Select(x => new ConstructorMemberInfo
            {
                Name = x.Declaration.Variables.First().Identifier.ValueText,
                Type = x.Declaration.Type,
            });

        var propertyDeclarations = memberDeclarations
            .OfType<PropertyDeclarationSyntax>()
            .Select(x => new ConstructorMemberInfo
            {
                Name = x.Identifier.ValueText,
                Type = x.Type
            });

        return fieldDeclarations
            .Concat(propertyDeclarations)
            .ToArray();
    }
}
