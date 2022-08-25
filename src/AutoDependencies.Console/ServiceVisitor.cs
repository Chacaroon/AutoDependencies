using AutoDependencies.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.ConsoleApp;

class ServiceVisitor : CSharpSyntaxWalker
{
    private readonly ServiceManager _serviceManager;

    public ServiceVisitor(ServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    public List<ClassDeclarationSyntax> ServiceNodes { get; } = new();
        
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (_serviceManager.IsApplicableForSourceGeneration(node))
        {
            ServiceNodes.Add(node);
        }
    }
}