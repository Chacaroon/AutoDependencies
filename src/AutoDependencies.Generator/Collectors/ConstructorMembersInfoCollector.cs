using AutoDependencies.Generator.Extensions;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Collectors;
internal static class ConstructorMembersInfoCollector
{
    public static ConstructorInfo GetConstructorMembersInfo(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel)
    {
        var constructorMembers = classDeclarationSyntax.DescendantNodes()
            .OfType<MemberDeclarationSyntax>()
            .Where(x => CanBeConstructorMember(x, semanticModel))
            .Select(x => CreateConstructorMemberInfo(x, semanticModel))
            .Where(x => x != null)
            .ToArray();

        var externalConstructorMembers = ExternalConstructorInfoCollector
            .ExternalConstructorInfo(classDeclarationSyntax, semanticModel);

        return new(constructorMembers, externalConstructorMembers);
    }

    private static bool CanBeConstructorMember(
        MemberDeclarationSyntax memberDeclarationSyntax,
        SemanticModel semanticModel)
    {
        if (memberDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return false;
        }

        if (memberDeclarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword)
            && memberDeclarationSyntax.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
        {
            return true;
        }

        var hasInitializer = memberDeclarationSyntax switch
        {
            FieldDeclarationSyntax fieldDeclarationSyntax => fieldDeclarationSyntax.Declaration.Variables.First().Initializer != null,
            PropertyDeclarationSyntax { Initializer: null } => false,
            _ => true
        };

        return !hasInitializer 
               && memberDeclarationSyntax.HasAttribute(AttributeNames.InjectAttribute, semanticModel);
    }

    private static ConstructorMemberInfo CreateConstructorMemberInfo(
        MemberDeclarationSyntax declarationSyntax,
        SemanticModel semanticModel)
    {
        var (identifier, type) = declarationSyntax switch
        {
            FieldDeclarationSyntax fieldDeclarationSyntax => (
                fieldDeclarationSyntax.Declaration.Variables.First().Identifier,
                fieldDeclarationSyntax.Declaration.Type),
            
            PropertyDeclarationSyntax propertyDeclarationSyntax => (
                propertyDeclarationSyntax.Identifier,
                propertyDeclarationSyntax.Type),

            _ => (default, default)
        };

        if (identifier == default || type == default)
        {
            return null!;
        }

        return new ConstructorMemberInfo(Name: identifier.Text, Type: type.ToFullNameTypeSyntax(semanticModel));
    }
}
