using AutoDependencies.Generator.Extensions;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Collectors;

public static class ExternalConstructorInfoCollector
{
    public static ConstructorMemberInfo[] ExternalConstructorInfo(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel)
    {
        var constructor = classDeclarationSyntax
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault();

        if (constructor == null)
        {
            return Array.Empty<ConstructorMemberInfo>();
        }

        return constructor
            .ParameterList
            .Parameters
            .Where(x => x.Type != null)
            .Select(x => new ConstructorMemberInfo(x.Identifier.Text, x.Type!.ToFullNameTypeSyntax(semanticModel)))
            .ToArray();
    } 
}