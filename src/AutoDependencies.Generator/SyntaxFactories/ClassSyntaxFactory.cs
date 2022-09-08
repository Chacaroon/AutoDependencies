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
        var interfaceIdentifier = IdentifierName(serviceInfo.InterfaceName);
        var interfaceList = BaseList(SeparatedList(new BaseTypeSyntax[]
        {
            SimpleBaseType(interfaceIdentifier)
        }));
        
        var classDeclaration = ClassDeclaration(serviceInfo.Name)
            .WithAttributeLists(List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(GeneratorConstants.AttributeNames.GeneratedAttribute)
            }))
            .WithModifiers(serviceInfo.Modifiers)
            .WithBaseList(interfaceList)
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                constructor
            }));

        return classDeclaration;
    }
}
