using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
internal static class InterfaceSyntaxFactory
{
    public static InterfaceDeclarationSyntax CreateInterfaceDeclarationSyntax(
        IdentifierNameSyntax interfaceName,
        InterfaceMemberInfo[] interfaceMembersInfo)
    {
        var interfaceDeclaration = InterfaceDeclaration(interfaceName.Identifier)
            .WithAttributeLists(List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(AttributeNames.GeneratedAttribute)
            }))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithMembers(CreateInterfaceMembersSyntax(interfaceMembersInfo));

        return interfaceDeclaration;
    }

    private static SyntaxList<MemberDeclarationSyntax> CreateInterfaceMembersSyntax(
        InterfaceMemberInfo[] interfaceMembersInfo)
    {
        var members = interfaceMembersInfo
            .Select(x => MethodDeclaration(x.ReturnType, x.Name)
                .WithParameterList(x.ParameterList)
                .WithBody(null)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))
            .Cast<MemberDeclarationSyntax>()
            .ToArray();

        return List(members);
    }
}
