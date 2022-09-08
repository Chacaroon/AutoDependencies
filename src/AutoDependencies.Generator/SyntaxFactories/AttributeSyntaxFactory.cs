using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using AutoDependencies.Generator.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
public static class AttributeSyntaxFactory
{
    private static readonly ConcurrentDictionary<string, AttributeSyntax> Attributes = new();
    private static readonly ConcurrentDictionary<string, AttributeListSyntax> AttributeLists = new();

    public static CompilationUnitSyntax GetOrCreateAttributeDeclarationSyntax(
        string attributeName,
        AttributeTargets[] attributeTargets,
        string namespaceName)
    {
        var attributeUsageAttributeList = CreateAttributeUsageAttributeListsDeclarationSyntax(attributeTargets);

        var attributeClassDeclaration = ClassDeclaration(attributeName)
            .WithBaseList(BaseList(SeparatedList(new BaseTypeSyntax[]
            {
                SimpleBaseType(IdentifierName(nameof(Attribute)))
            })))
            .WithAttributeLists(
                List(new[]
                {
                    GetOrCreateAttributeListSyntax(GeneratorConstants.AttributeNames.GeneratedAttribute),
                    attributeUsageAttributeList
                }));

        var namespaceDeclaration = NamespaceSyntaxFactory.CreateNamespace(namespaceName, new[] { attributeClassDeclaration });

        var root = CompilationUnit()
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                namespaceDeclaration
            }))
            .WithUsings(List(new[]
            {
                UsingDirective(IdentifierName(nameof(System))),
            }))
            .NormalizeWhitespace();

        return root;
    }

    public static AttributeSyntax GetOrCreateAttributeSyntax(string attributeName)
    {
        attributeName = NormalizeAttributeName(attributeName);
        if (Attributes.TryGetValue(attributeName, out var attributeSyntax))
        {
            return attributeSyntax;
        }

        var identifier = IdentifierName(attributeName);

        return Attributes[attributeName] = Attribute(identifier);
    }

    public static AttributeListSyntax GetOrCreateAttributeListSyntax(string attributeName)
    {
        attributeName = NormalizeAttributeName(attributeName);
        if (AttributeLists.TryGetValue(attributeName, out var attributeListSyntax))
        {
            return attributeListSyntax;
        }

        return AttributeLists[attributeName] = AttributeList(SeparatedList(new[]
        {
            GetOrCreateAttributeSyntax(attributeName)
        }));
    }

    private static AttributeListSyntax CreateAttributeUsageAttributeListsDeclarationSyntax(AttributeTargets[] attributeTargets)
    {
        if (attributeTargets.Length == 0)
        {
            attributeTargets = new[] { AttributeTargets.All };
        }
        
        var attributeUsageParam = attributeTargets.Skip(1).Aggregate(
            CreateAttributeTargetExpression(attributeTargets[0]),
            (syntax, targets) => BinaryExpression(
                SyntaxKind.BitwiseOrExpression,
                syntax,
                CreateAttributeTargetExpression(targets)));

        var arguments = AttributeArgumentList(
            SeparatedList(new[]
            {
                AttributeArgument(attributeUsageParam)
            }));

        var attributeUsageAttribute = GetOrCreateAttributeSyntax(nameof(AttributeUsageAttribute))
            .WithArgumentList(arguments);

        return AttributeList(SeparatedList(new[]
        {
            attributeUsageAttribute
        }));
    }

    private static ExpressionSyntax CreateAttributeTargetExpression(AttributeTargets attributeTargets)
    {
        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(nameof(AttributeTargets)),
            IdentifierName(attributeTargets.ToString()));
    }

    private static string NormalizeAttributeName(string attributeName)
    {
        return Regex.Replace(attributeName, "Attribute$", string.Empty);
    }
}
