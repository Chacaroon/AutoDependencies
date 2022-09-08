using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Collectors;
public static class ServiceCollector
{
    public static ServiceToGenerateInfo GetServiceToGenerateInfo(
        ClassDeclarationSyntax classDeclarationSyntax,
        SemanticModel semanticModel, 
        bool nullableEnabled)
    {
        return new(
            ServiceInfoCollector.GetServiceInfo(classDeclarationSyntax),
            InterfaceMembersInfoCollector.GetInterfaceMembersInfo(classDeclarationSyntax, semanticModel),
            ConstructorMembersInfoCollector.GetConstructorMembersInfo(classDeclarationSyntax, semanticModel),
            nullableEnabled);
    }
}
