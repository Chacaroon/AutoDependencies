using AutoDependencies.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.ConsoleApp;

class ServiceVisitor : CSharpSyntaxWalker
{
    private readonly ServiceAnalyzer _serviceAnalyzer;

    public ServiceVisitor(ServiceAnalyzer serviceAnalyzer)
    {
        _serviceAnalyzer = serviceAnalyzer;
    }

    public List<ClassDeclarationSyntax> ServiceNodes { get; } = new();
        
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (!ServiceAnalyzer.IsCandidateForGeneration(node))
        {
            return;
        }

        if (!_serviceAnalyzer.IsApplicableForSourceGeneration(node))
        {
            return;
        }

        ServiceNodes.Add(node);
    }
}