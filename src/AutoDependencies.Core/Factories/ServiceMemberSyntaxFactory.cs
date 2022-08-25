using System.Text.RegularExpressions;
using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core.Factories;

internal static class ServiceMemberSyntaxFactory
{
    private static readonly Regex UnderscoreRegex = new("^_", RegexOptions.Compiled);

    public static ParameterSyntax[] CreateConstructorParameters(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var descendantNodes = classDeclarationSyntax
            .DescendantNodes()
            .ToArray();

        var properties = descendantNodes
            .OfType<PropertyDeclarationSyntax>()
            .Select(x => (x.Type, Name: ProcessMemberName(x.Identifier.Text)))
            .ToArray();

        var fields = descendantNodes
            .OfType<FieldDeclarationSyntax>()
            .Select(x => x.Declaration)
            .Select(x => (x.Type, Name: ProcessMemberName(x.Variables.Single().Identifier.Text)))
            .ToArray();

        return properties
            .Concat(fields)
            .Select(x => SyntaxFactory.Parameter(SyntaxFactory.Identifier(x.Name)).WithType(x.Type))
            .ToArray();
    }

    public static StatementSyntax[] CreateAssignmentStatements(
        ClassDeclarationSyntax classDeclarationSyntax,
        SemanticModel semanticModel)
    {
        var descendantNodes = GetMembersForProcessing(classDeclarationSyntax, semanticModel);

        var properties = descendantNodes
            .OfType<PropertyDeclarationSyntax>()
            .Select(x => x.Identifier);

        var fields = descendantNodes
            .OfType<FieldDeclarationSyntax>()
            .Select(x => x.Declaration)
            .Select(x => x.Variables.Single().Identifier);

        var members = properties.Concat(fields);

        var expressionStatements = members
            .Select(x => SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(x),
                SyntaxFactory.IdentifierName(ProcessMemberName(x.Text))))
            .Select(SyntaxFactory.ExpressionStatement)
            .Cast<StatementSyntax>()
            .ToArray();

        return expressionStatements;
    }

    private static MemberDeclarationSyntax[] GetMembersForProcessing(
        ClassDeclarationSyntax classDeclarationSyntax,
        SemanticModel semanticModel)
    {
        return classDeclarationSyntax
            .DescendantNodes()
            .OfType<MemberDeclarationSyntax>()
            .Where(x => x.HasAttribute(CoreConstants.InjectAttributeName, semanticModel))
            .ToArray();
    }

    private static string ProcessMemberName(string identifier)
    {
        identifier = UnderscoreRegex.Replace(identifier, string.Empty);

        return identifier.Length > 1
            ? $"{char.ToLower(identifier[0])}{identifier.Substring(1)}"
            : identifier.ToLower();
    }
}
