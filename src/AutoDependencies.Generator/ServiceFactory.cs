﻿using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Factories;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator;
public class ServiceFactory
{
    public SyntaxNode GenerateService(ServiceToGenerateInfo serviceToGenerateInfo)
    {
        var (serviceInfo, interfaceMembersInfo, constructorMembersInfo) = serviceToGenerateInfo;

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
                Constants.GeneratorConstants.AttributesNamespace
            }));

        return root.NormalizeWhitespace();
    }
}