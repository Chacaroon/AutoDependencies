﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using AutoDependencies.Core.Constants;

namespace AutoDependencies.Core.Factories;
internal static class NamespaceSyntaxFactory
{
    public static NamespaceDeclarationSyntax CreateNamespace(string namespaceName, IEnumerable<MemberDeclarationSyntax>? members = null)
    {
        var namespaceIdentifier = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(namespaceName));

        var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(namespaceIdentifier)
            .WithMembers(SyntaxFactory.List(members ?? Array.Empty<MemberDeclarationSyntax>()));

        return namespaceDeclaration;
    }
}