using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
internal static class InterfaceSyntaxFactory
{
    public static InterfaceDeclarationSyntax CreateInterfaceDeclarationSyntax(
        string interfaceName,
        InterfaceMemberInfo[] interfaceMembersInfo)
    {
        var interfaceDeclaration = InterfaceDeclaration(interfaceName)
            .WithAttributeLists(List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(GeneratorConstants.AttributeNames.GeneratedAttribute)
            }))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithMembers(CreateInterfaceMembers(interfaceMembersInfo));

        return interfaceDeclaration;
    }

    private static SyntaxList<MemberDeclarationSyntax> CreateInterfaceMembers(
        InterfaceMemberInfo[] interfaceMembersInfo)
    {
        var members = interfaceMembersInfo
            .Select(x => MethodDeclaration(x.ReturnType, x.Name)
                .WithModifiers(TokenList())
                .WithBody(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))
            .Cast<MemberDeclarationSyntax>()
            .ToArray();

        return List(members);
    }
}
