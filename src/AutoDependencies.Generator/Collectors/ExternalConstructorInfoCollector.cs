using AutoDependencies.Generator.Extensions;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Collectors;

public static class ExternalConstructorInfoCollector
{
    public static ConstructorMemberInfo[] ExternalConstructorInfo(
        ClassDeclarationSyntax classDeclarationSyntax,
        SemanticModel semanticModel)
    {
        var constructors = classDeclarationSyntax
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>()
            .ToArray();

        var constructor = constructors switch
        {
            { Length: 1 } => constructors[0],
            { Length: > 1} => constructors.FirstOrDefault(x =>
                x.HasAttribute(AttributeNames.ServiceConstructorAttribute, semanticModel)),
            _ => null
        };

        return constructor?
            .ParameterList
            .Parameters
            .Where(x => x.Type != null)
            .Select(x => new ConstructorMemberInfo(x.Identifier.Text, x.Type!.ToFullNameTypeSyntax(semanticModel)))
            .ToArray()
            ?? Array.Empty<ConstructorMemberInfo>();
    }
}