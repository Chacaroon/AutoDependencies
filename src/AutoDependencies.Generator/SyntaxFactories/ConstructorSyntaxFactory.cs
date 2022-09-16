using System.Text.RegularExpressions;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
internal static class ConstructorSyntaxFactory
{
    private static readonly Regex UnderscoreRegex = new("^_", RegexOptions.Compiled);

    public static ConstructorDeclarationSyntax CreateConstructorSyntax(
        ServiceInfo serviceInfo,
        ConstructorInfo constructorInfo)
    {
        var (constructorMembers, externalConstructorMembers) = constructorInfo;
        var constructorParameters = constructorMembers
            .Concat(externalConstructorMembers)
            .Distinct()
            .ToArray();
        var parameters = CreateConstructorParametersSyntax(constructorParameters);
        var body = Block(CreateAssignmentStatementsSyntax(constructorMembers));

        var constructorDeclaration = ConstructorDeclaration(serviceInfo.ServiceName)
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(parameters)
            .WithBody(body);

        if (constructorInfo.HasExternalConstructor)
        {
            constructorDeclaration = constructorDeclaration.WithInitializer(
                ConstructorInitializer(SyntaxKind.ThisConstructorInitializer, CreateExternalConstructorArgumentsSyntax(externalConstructorMembers)));
        }

        return constructorDeclaration;
    }

    private static ParameterListSyntax CreateConstructorParametersSyntax(ConstructorMemberInfo[] constructorMembersInfo)
    {
        if (constructorMembersInfo.Length == 0)
        {
            return ParameterList();
        }

        var parameters = constructorMembersInfo
            .Select(x => (x.Type, Name: NormalizeMemberName(x.Name)))
            .Select(x => Parameter(Identifier(x.Name))
                .WithType(x.Type))
            .ToArray();

        return ParameterList(SeparatedList(parameters));
    }

    private static ArgumentListSyntax CreateExternalConstructorArgumentsSyntax(ConstructorMemberInfo[] constructorMemberInfos)
    {
        var arguments = constructorMemberInfos
            .Select(x => Argument(IdentifierName(NormalizeMemberName(x.Name))))
            .ToArray();

        return ArgumentList(SeparatedList(arguments));
    }

    private static StatementSyntax[] CreateAssignmentStatementsSyntax(ConstructorMemberInfo[] constructorMembersInfo)
    {
        var expressionStatements = constructorMembersInfo
            .Select(x => AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(x.Name),
                IdentifierName(NormalizeMemberName(x.Name))))
            .Select(ExpressionStatement)
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
