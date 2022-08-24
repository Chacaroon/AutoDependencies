using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core;
public static class AttributesManager
{
    private static readonly Dictionary<string, AttributeSyntax> Attributes = new();
    private static readonly Dictionary<string, AttributeListSyntax> AttributeLists = new();

    public static CompilationUnitSyntax GetOrCreateAttributeDeclaration(
        string attributeName,
        AttributeTargets[] attributeTargets,
        string namespaceName)
    {
        var attributeUsageAttributeList = CreateAttributeUsageAttributeListsDeclaration(attributeTargets);

        var attributeClassDeclaration = SyntaxFactory.ClassDeclaration(attributeName)
            .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(new BaseTypeSyntax[]
            {
                SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(nameof(Attribute)))
            })))
            .WithAttributeLists(
                SyntaxFactory.List(new[]
                {
                    GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName),
                    attributeUsageAttributeList
                }));

        var namespaceDeclaration = SyntaxNodesFactory.CreateNamespace(namespaceName, new[] { attributeClassDeclaration });

        var root = SyntaxFactory.CompilationUnit()
            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                namespaceDeclaration
            }))
            .WithUsings(SyntaxFactory.List(new[]
            {
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(nameof(System)))
            }));

        return root;
    }

    public static AttributeSyntax GetOrCreateAttributeSyntax(string attributeName)
    {
        attributeName = NormalizeAttributeName(attributeName);
        if (Attributes.TryGetValue(attributeName, out var attributeSyntax))
        {
            return attributeSyntax;
        }

        var identifier = SyntaxFactory.IdentifierName(attributeName);

        return Attributes[attributeName] = SyntaxFactory.Attribute(identifier);
    }

    public static AttributeListSyntax GetOrCreateAttributeListSyntax(string attributeName)
    {
        attributeName = NormalizeAttributeName(attributeName);
        if (AttributeLists.TryGetValue(attributeName, out var attributeListSyntax))
        {
            return attributeListSyntax;
        }

        return AttributeLists[attributeName] = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[]
        {
            GetOrCreateAttributeSyntax(attributeName)
        }));
    }

    private static AttributeListSyntax CreateAttributeUsageAttributeListsDeclaration(AttributeTargets[] attributeTargets)
    {
        if (attributeTargets.Length == 0)
        {
            attributeTargets = new[] { AttributeTargets.All };
        }

        var attributeTargetsExpression = CreateAttributeTargetExpression(attributeTargets[0]);

        var attributeUsageParam = attributeTargets.Skip(1).Aggregate(
            attributeTargetsExpression,
            (syntax, targets) => SyntaxFactory.BinaryExpression(
                SyntaxKind.BitwiseOrExpression,
                syntax,
                CreateAttributeTargetExpression(targets)));

        var arguments = SyntaxFactory.AttributeArgumentList(
            SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.AttributeArgument(attributeUsageParam)
            }));

        const string attributeUsageName = nameof(AttributeUsageAttribute);

        var attributeUsageAttribute = SyntaxFactory.Attribute(
            SyntaxFactory.IdentifierName(attributeUsageName),
            arguments);

        return SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[]
        {
            attributeUsageAttribute
        }));
    }

    private static ExpressionSyntax CreateAttributeTargetExpression(AttributeTargets attributeTargets)
    {
        return SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName(nameof(AttributeTargets)),
            SyntaxFactory.IdentifierName(attributeTargets.ToString()));
    }

    private static string NormalizeAttributeName(string attributeName)
    {
        return Regex.Replace(attributeName, "Attribute$", string.Empty);
    }
}
