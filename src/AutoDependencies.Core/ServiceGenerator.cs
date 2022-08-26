using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using AutoDependencies.Core.Factories;
using AutoDependencies.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core;
public class ServiceGenerator
{
    public SyntaxNode GenerateService(
        ServiceInfo serviceInfo,
        ConstructorMemberInfo[] constructorMembersInfo,
        InterfaceMemberInfo[] interfaceMembersInfo)
    {
        var interfaceDeclaration = InterfaceSyntaxFactory.CreateInterfaceDeclarationSyntax(serviceInfo.InterfaceName, interfaceMembersInfo);
        var constructorDeclarationSyntax = ConstructorSyntaxFactory.CreateConstructor(serviceInfo, constructorMembersInfo);

        var classDeclaration = ClassSyntaxFactory.GeneratePartialClassService(serviceInfo, constructorDeclarationSyntax);

        var namespaceDeclaration = NamespaceSyntaxFactory.CreateNamespace(
            serviceInfo.Namespace, 
            new MemberDeclarationSyntax[] { classDeclaration, interfaceDeclaration });

        var root = SyntaxFactory.CompilationUnit()
            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                namespaceDeclaration
            }))
            .WithUsings(UsingSyntaxFactory.CreateUsingDirectiveList(new[]
            {
                CoreConstants.AttributesNamespace
            }));

        return root;
    }
}
