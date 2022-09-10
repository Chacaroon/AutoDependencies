using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
public static class ServiceRegistrationSyntaxFactory
{
    public static StatementSyntax CreateServiceRegistrationCallSyntax(
        ServiceInfo serviceInfo,
        SyntaxToken servicesIdentifier,
        SyntaxToken lifetimeIdentifier)
    {
        var methodAccessExpressionSyntax = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(servicesIdentifier),
            IdentifierName("Add"));

        var argumentListSyntax = ArgumentList(
            SeparatedList(new[]
            {
                Argument(TypeOfExpression(IdentifierName(serviceInfo.InterfaceName))),
                Argument(TypeOfExpression(IdentifierName(serviceInfo.ServiceName))),
                Argument(IdentifierName(lifetimeIdentifier))
            }));

        var createServiceDescriptorExpression = ObjectCreationExpression(IdentifierName("ServiceDescriptor"))
            .WithArgumentList(argumentListSyntax);

        var methodCallSyntax = InvocationExpression(methodAccessExpressionSyntax, ArgumentList(
            SeparatedList(new[]
            {
                Argument(createServiceDescriptorExpression)
            })));

        var expressionStatementSyntax = ExpressionStatement(methodCallSyntax);

        return expressionStatementSyntax;
    }
}
