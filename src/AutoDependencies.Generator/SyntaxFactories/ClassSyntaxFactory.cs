using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
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
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(GeneratorConstants.AttributeNames.GeneratedAttribute)
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
