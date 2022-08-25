using AutoDependencies.Core.Constants;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace AutoDependencies.Core.Factories;
internal static class ClassSyntaxFactory
{
    public static ClassDeclarationSyntax GeneratePartialClassWithInterface(
        ClassDeclarationSyntax classDeclarationSyntax,
        IdentifierNameSyntax interfaceIdentifier,
        SemanticModel semanticModel)
    {
        var interfaceList = SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(new BaseTypeSyntax[]
        {
            SyntaxFactory.SimpleBaseType(interfaceIdentifier)
        }));

        var constructorParameters = ServiceMemberSyntaxFactory.CreateConstructorParameters(classDeclarationSyntax);
        var constructorParameterList = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(constructorParameters));

        var body = SyntaxFactory.Block(ServiceMemberSyntaxFactory.CreateAssignmentStatements(classDeclarationSyntax, semanticModel));

        var constructor = SyntaxFactory.ConstructorDeclaration(classDeclarationSyntax.Identifier)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(constructorParameterList)
            .WithBody(body);

        var classDeclaration = SyntaxFactory.ClassDeclaration(classDeclarationSyntax.Identifier)
            .WithAttributeLists(SyntaxFactory.List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName)
            }))
            .WithModifiers(classDeclarationSyntax.Modifiers)
            .WithBaseList(interfaceList)
            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                constructor
            }));

        return classDeclaration;
    }
}
