using AutoDependencies.Core.Constants;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDependencies.Core.Factories;
internal static class ClassServiceFactory
{
    public static ClassDeclarationSyntax GeneratePartialClassWithInterface(
        ClassDeclarationSyntax classDeclarationSyntax,
        IdentifierNameSyntax interfaceIdentifier)
    {
        var interfaceList = SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(new BaseTypeSyntax[]
        {
            SyntaxFactory.SimpleBaseType(interfaceIdentifier)
        }));

        var classDeclaration = SyntaxFactory.ClassDeclaration(classDeclarationSyntax.Identifier)
            .WithAttributeLists(SyntaxFactory.List(new[]
            {
                AttributeFactory.GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName)
            }))
            .WithModifiers(classDeclarationSyntax.Modifiers)
            .WithBaseList(interfaceList);

        return classDeclaration;
    }
}
