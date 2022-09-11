using AutoDependencies.Generator.Extensions;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Collectors;
internal static class InterfaceMembersInfoCollector
{
    public static InterfaceInfo GetInterfaceMembersInfo(
        ClassDeclarationSyntax classDeclarationSyntax,
        SemanticModel semanticModel)
    {
        var members = classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Where(CanBeInterfaceMember)
            .Select(x => new InterfaceMemberInfo(Name: x.Identifier.ValueText,
                ParameterList: MapParameterTypesToFullNameTypeSyntax(x.ParameterList, semanticModel),
                ReturnType: x.ReturnType.ToFullNameTypeSyntax(semanticModel)))
            .ToArray();

        var namespaceName = $"{semanticModel.Compilation.AssemblyName ?? "AutoDependencies"}.Interfaces.Generated";

        return new($"I{classDeclarationSyntax.Identifier}", namespaceName, members);
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
