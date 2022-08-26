using System;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using AutoDependencies.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core.Factories;
internal static class InterfaceSyntaxFactory
{
    public static InterfaceDeclarationSyntax CreateInterfaceDeclarationSyntax(
        string interfaceName,
        InterfaceMemberInfo[] interfaceMembersInfo)
    {
        var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName)
            .WithAttributeLists(SyntaxFactory.List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName)
            }))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithMembers(CreateInterfaceMembers(interfaceMembersInfo));

        return interfaceDeclaration;
    }

    private static SyntaxList<MemberDeclarationSyntax> CreateInterfaceMembers(
        InterfaceMemberInfo[] interfaceMembersInfo)
    {
        var members = interfaceMembersInfo
            .Select(x => SyntaxFactory.MethodDeclaration(x.ReturnType, x.Name)
                .WithModifiers(SyntaxFactory.TokenList())
                .WithBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
            .Cast<MemberDeclarationSyntax>()
            .ToArray();

        return SyntaxFactory.List(members);
    }
}
