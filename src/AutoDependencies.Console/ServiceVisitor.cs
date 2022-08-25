using AutoDependencies.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.ConsoleApp;

class ServiceVisitor : CSharpSyntaxWalker
{
    private readonly ServiceGenerator _serviceGenerator;

    public ServiceVisitor(ServiceGenerator serviceGenerator)
    {
        _serviceGenerator = serviceGenerator;
    }

    public List<ClassDeclarationSyntax> ServiceNodes { get; } = new();
        
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (_serviceGenerator.IsApplicableForSourceGeneration(node))
        {
            ServiceNodes.Add(node);
        }
    }
}