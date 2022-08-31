using AutoDependencies.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.ConsoleApp;

class ServiceVisitor : CSharpSyntaxWalker
{
    private readonly ServiceAnalyzer _serviceAnalyzer;
    private readonly SemanticModel _semanticModel;

    public ServiceVisitor(ServiceAnalyzer serviceAnalyzer, SemanticModel semanticModel)
    {
        _serviceAnalyzer = serviceAnalyzer;
        _semanticModel = semanticModel;
    }

    public List<ClassDeclarationSyntax> ServiceNodes { get; } = new();
        
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (!ServiceAnalyzer.IsCandidateForGeneration(node))
        {
            return;
        }

        if (!ServiceAnalyzer.IsApplicableForSourceGeneration(node, _semanticModel))
        {
            return;
        }

        ServiceNodes.Add(node);
    }
}