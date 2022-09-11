using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
internal static class ClassSyntaxFactory
{
    public static ClassDeclarationSyntax GeneratePartialClassServiceSyntax(
        ServiceInfo serviceInfo,
        InterfaceInfo interfaceInfo,
        ConstructorDeclarationSyntax constructor)
    {
        var interfaceList = BaseList(SeparatedList(new BaseTypeSyntax[]
        {
            SimpleBaseType(IdentifierName(interfaceInfo.InterfaceName))
        }));
        
        var classDeclaration = ClassDeclaration(serviceInfo.ServiceName)
            .WithAttributeLists(List(new[]
            {
                AttributeSyntaxFactory.GetOrCreateAttributeListSyntax(AttributeNames.GeneratedAttribute)
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
