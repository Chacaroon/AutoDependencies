using System;
using System.Collections.Generic;
using System.Text;
using AutoDependencies.Core.Constants;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core.Factories;
internal static class InterfaceServiceFactory
{
    public static (InterfaceDeclarationSyntax, IdentifierNameSyntax) CreateInterfaceForClass(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var interfaceName = $"I{classDeclarationSyntax.Identifier.Text}";
        var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration(interfaceName)
            .WithAttributeLists(SyntaxFactory.List(new[]
            {
                AttributeFactory.GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName)
            }))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

        var interfaceIdentifier = SyntaxFactory.IdentifierName(interfaceName);

        return (interfaceDeclaration, interfaceIdentifier);
    }
}
