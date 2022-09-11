using AutoDependencies.Generator.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.SyntaxFactories;
public static class ServiceRegistrationSyntaxFactory
{
    private static readonly IdentifierNameSyntax AddMethodIdentifier = IdentifierName("Add");
    private static readonly ObjectCreationExpressionSyntax ServiceDescriptorCreationSyntax =
        ObjectCreationExpression(IdentifierName("ServiceDescriptor"));

    public static StatementSyntax CreateServiceRegistrationCallSyntax(
        ServiceInfo serviceInfo,
        InterfaceInfo interfaceInfo,
        IdentifierNameSyntax servicesIdentifier,
        IdentifierNameSyntax lifetimeIdentifier)
    {
        var methodAccessExpressionSyntax = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            servicesIdentifier,
            AddMethodIdentifier);

        var argumentListSyntax = ArgumentList(
            SeparatedList(new[]
            {
                Argument(TypeOfExpression(IdentifierName(interfaceInfo.InterfaceName))),
                Argument(TypeOfExpression(IdentifierName(serviceInfo.ServiceName))),
                Argument(lifetimeIdentifier)
            }));

        var createServiceDescriptorExpression = ServiceDescriptorCreationSyntax
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
