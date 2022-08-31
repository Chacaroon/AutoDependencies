using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Extensions;
using AutoDependencies.Core.Models;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoDependencies.Core;
public class ServiceAnalyzer
{
    private readonly SemanticModel _semanticModel;

    private static readonly SyntaxKind[] ForbiddenModifiers = {
        SyntaxKind.StaticKeyword,
        SyntaxKind.AbstractKeyword
    };

    public ServiceAnalyzer(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public static bool IsCandidateForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    public static bool IsApplicableForSourceGeneration(ClassDeclarationSyntax node, SemanticModel semanticModel)
    {
        if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        if (ForbiddenModifiers.Any(x => node.Modifiers.Any(x)))
        {
            return false;
        }

        return node.HasAttribute(CoreConstants.ServiceAttributeName, semanticModel);
    }

    public ServiceToGenerateInfo GetServiceToGenerateInfo(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return new(
            GetServiceInfo(classDeclarationSyntax),
            GetInterfaceMembersInfo(classDeclarationSyntax),
            GetConstructorMembersInfo(classDeclarationSyntax));
    }

    private ServiceInfo GetServiceInfo(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var namespaceName = _semanticModel
            .GetDeclaredSymbol(classDeclarationSyntax)!
            .ContainingNamespace
            .ToDisplayString();

        return new ServiceInfo
        {
            Name = classDeclarationSyntax.Identifier,
            Modifiers = classDeclarationSyntax.Modifiers,
            Namespace = namespaceName
        };
    }

    private InterfaceMemberInfo[] GetInterfaceMembersInfo(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .Where(x => x.Modifiers.Any(SyntaxKind.PublicKeyword)
                        && !x.Modifiers.Any(SyntaxKind.StaticKeyword))
            .Select(x => new InterfaceMemberInfo
            {
                Name = x.Identifier.ValueText,
                ParameterList = x.ParameterList,
                ReturnType = x.ReturnType.ToFullNameTypeSyntax(_semanticModel),
            })
            .ToArray();
    }

    private ConstructorMemberInfo[] GetConstructorMembersInfo(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var memberDeclarations = classDeclarationSyntax.DescendantNodes()
            .OfType<MemberDeclarationSyntax>()
            .Where(CanBeConstructorMember)
            .ToArray();

        var fieldDeclarations = memberDeclarations
            .OfType<FieldDeclarationSyntax>()
            .Select(x => (VariableDeclaration: x.Declaration.Variables.First(), x.Declaration.Type))
            .Where(x => x.VariableDeclaration.Initializer == null)
            .Select(x => CreateConstructorMemberInfo(x.VariableDeclaration.Identifier, x.Type));

        var propertyDeclarations = memberDeclarations
            .OfType<PropertyDeclarationSyntax>()
            .Where(x => x.Initializer == null)
            .Select(x => CreateConstructorMemberInfo(x.Identifier, x.Type));

        return fieldDeclarations
            .Concat(propertyDeclarations)
            .ToArray();
    }

    private bool CanBeConstructorMember(MemberDeclarationSyntax memberDeclarationSyntax)
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

        return memberDeclarationSyntax.HasAttribute(CoreConstants.InjectAttributeName, _semanticModel);
    }

    private ConstructorMemberInfo CreateConstructorMemberInfo(SyntaxToken identifier, TypeSyntax type)
    {
        return new ConstructorMemberInfo
        {
            Name = identifier.Text,
            Type = type.ToFullNameTypeSyntax(_semanticModel)
        };
    }
}
