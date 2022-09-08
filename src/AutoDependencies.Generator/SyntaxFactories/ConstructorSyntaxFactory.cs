using System.Text.RegularExpressions;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
internal class ConstructorSyntaxFactory
{
    private static readonly Regex UnderscoreRegex = new("^_", RegexOptions.Compiled);

    public static ConstructorDeclarationSyntax CreateConstructor(
        ServiceInfo serviceInfo,
        ConstructorMemberInfo[] constructorMembersInfo)
    {
        var parameters = CreateConstructorParameters(constructorMembersInfo);
        var body = SyntaxFactory.Block(CreateAssignmentStatements(constructorMembersInfo));

        var constructorDeclaration = SyntaxFactory.ConstructorDeclaration(serviceInfo.Name)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(parameters)
            .WithBody(body);

        return constructorDeclaration;
    }

    private static ParameterListSyntax CreateConstructorParameters(ConstructorMemberInfo[] constructorMembersInfo)
    {
        if (constructorMembersInfo.Length == 0)
        {
            return SyntaxFactory.ParameterList();
        }

        var parameters = constructorMembersInfo
            .Select(x => (x.Type, Name: NormalizeMemberName(x.Name)))
            .Select(x => SyntaxFactory
                .Parameter(SyntaxFactory.Identifier(x.Name))
                .WithType(x.Type))
            .ToArray();
        

        return SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters));
    }

    private static StatementSyntax[] CreateAssignmentStatements(ConstructorMemberInfo[] constructorMembersInfo)
    {
        var expressionStatements = constructorMembersInfo
            .Select(x => SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(x.Name),
                SyntaxFactory.IdentifierName(NormalizeMemberName(x.Name))))
            .Select(SyntaxFactory.ExpressionStatement)
            .Cast<StatementSyntax>()
            .ToArray();

        return expressionStatements;
    }

    private static string NormalizeMemberName(string identifier)
    {
        identifier = UnderscoreRegex.Replace(identifier, string.Empty);

        return identifier.Length > 1
            ? $"{char.ToLower(identifier[0])}{identifier.Substring(1)}"
            : identifier.ToLower();
    }
}
