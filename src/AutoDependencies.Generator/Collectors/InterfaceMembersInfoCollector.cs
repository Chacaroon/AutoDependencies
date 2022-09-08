using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using AutoDependencies.Generator.Extensions;

namespace AutoDependencies.Generator.Collectors;
internal static class InterfaceMembersInfoCollector
{
    public static InterfaceMemberInfo[] GetInterfaceMembersInfo(
        ClassDeclarationSyntax classDeclarationSyntax,
        SemanticModel semanticModel)
    {
        return classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Where(CanBeInterfaceMember)
            .Select(x => new InterfaceMemberInfo
            {
                Name = x.Identifier.ValueText,
                ParameterList = MapParameterTypesToFullNameTypeSyntax(x.ParameterList, semanticModel),
                ReturnType = x.ReturnType.ToFullNameTypeSyntax(semanticModel),
            })
            .ToArray();
    }

    private static bool CanBeInterfaceMember(MethodDeclarationSyntax syntax) =>
        syntax.Modifiers.Any(SyntaxKind.PublicKeyword)
        && !syntax.Modifiers.Any(SyntaxKind.StaticKeyword);

    private static ParameterListSyntax MapParameterTypesToFullNameTypeSyntax(
        ParameterListSyntax parameterListSyntax,
        SemanticModel semanticModel)
    {
        var parametersWithConvertedType = parameterListSyntax.Parameters
            .Select(x => x.WithType(x.Type!.ToFullNameTypeSyntax(semanticModel)));

        return ParameterList(SeparatedList(parametersWithConvertedType));
    }
}
