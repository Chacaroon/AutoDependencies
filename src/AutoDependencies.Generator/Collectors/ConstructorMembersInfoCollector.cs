using AutoDependencies.Generator.Constants;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using AutoDependencies.Generator.Extensions;

namespace AutoDependencies.Generator.Collectors;
internal static class ConstructorMembersInfoCollector
{
    public static ConstructorMemberInfo[] GetConstructorMembersInfo(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel)
    {
        var memberDeclarations = classDeclarationSyntax.DescendantNodes()
            .OfType<MemberDeclarationSyntax>()
            .Where(memberDeclarationSyntax => CanBeConstructorMember(memberDeclarationSyntax, semanticModel))
            .Select(x => CreateConstructorMemberInfo(x, semanticModel))
            .ToArray();
        
        return memberDeclarations;
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
               && memberDeclarationSyntax.HasAttribute(GeneratorConstants.AttributeNames.InjectAttribute, semanticModel);
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

            _ => throw new ArgumentOutOfRangeException(nameof(declarationSyntax), declarationSyntax, null)
        };

        return new ConstructorMemberInfo
        {
            Name = identifier.Text,
            Type = type.ToFullNameTypeSyntax(semanticModel)
        };
    }
}
