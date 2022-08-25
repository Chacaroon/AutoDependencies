using System;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core.Factories;
internal static class InterfaceSyntaxFactory
{
    public static InterfaceDeclarationSyntax CreateInterfaceDeclarationSyntax(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var interfaceName = $"I{classDeclarationSyntax.Identifier.Text}";

        var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName)
            .WithAttributeLists(SyntaxFactory.List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName)
            }))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithMembers(CreateInterfaceMembers(classDeclarationSyntax));

        return interfaceDeclaration;
    }

    private static SyntaxList<MemberDeclarationSyntax> CreateInterfaceMembers(
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        var result = classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Where(x => x.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword)))
            .Where(x => x.Modifiers.Any(x => !x.IsKind(SyntaxKind.StaticKeyword)))
            .Select(x => x
                .WithModifiers(SyntaxFactory.TokenList())
                .WithBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
            .Cast<MemberDeclarationSyntax>()
            .ToArray();

        return SyntaxFactory.List(result);
    }
}
