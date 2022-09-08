using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Collectors;

internal static class ServiceInfoCollector
{
    public static ServiceInfo GetServiceInfo(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var namespaceName = GetNamespace(classDeclarationSyntax);

        return new ServiceInfo
        {
            Name = classDeclarationSyntax.Identifier,
            Modifiers = classDeclarationSyntax.Modifiers,
            Namespace = namespaceName
        };
    }

    private static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        var @namespace = string.Empty;
        var potentialNamespaceParent = syntax.Parent;

        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        if (potentialNamespaceParent is not BaseNamespaceDeclarationSyntax namespaceParent)
        {
            return @namespace;
        }

        @namespace = namespaceParent.Name.ToString();

        while (true)
        {
            if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
            {
                break;
            }

            @namespace = $"{namespaceParent.Name}.{@namespace}";
            namespaceParent = parent;
        }

        return @namespace;
    }
}