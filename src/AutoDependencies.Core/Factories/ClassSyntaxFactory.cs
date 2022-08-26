using AutoDependencies.Core.Constants;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using AutoDependencies.Core.Models;

namespace AutoDependencies.Core.Factories;
internal static class ClassSyntaxFactory
{
    public static ClassDeclarationSyntax GeneratePartialClassService(
        ServiceInfo serviceInfo,
        ConstructorDeclarationSyntax constructor)
    {
        var interfaceIdentifier = SyntaxFactory.IdentifierName(serviceInfo.InterfaceName);
        var interfaceList = SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(new BaseTypeSyntax[]
        {
            SyntaxFactory.SimpleBaseType(interfaceIdentifier)
        }));
        
        var classDeclaration = SyntaxFactory.ClassDeclaration(serviceInfo.Name)
            .WithAttributeLists(SyntaxFactory.List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(CoreConstants.GeneratedAttributeName)
            }))
            .WithModifiers(serviceInfo.Modifiers)
            .WithBaseList(interfaceList)
            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                constructor
            }));

        return classDeclaration;
    }
}
