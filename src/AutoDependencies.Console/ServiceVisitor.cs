using AutoDependencies.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.ConsoleApp;

class ServiceVisitor : CSharpSyntaxWalker
{
    public List<ClassDeclarationSyntax> ServiceNodes { get; } = new();
        
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (ServiceManager.IsApplicableForSourceGeneration(node))
        {
            ServiceNodes.Add(node);
        }
    }
}