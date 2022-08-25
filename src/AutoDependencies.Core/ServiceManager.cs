using AutoDependencies.Core.Constants;
using AutoDependencies.Core.Factories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Core;
public class ServiceManager
{
    private readonly SemanticModel _semanticModel;

    public ServiceManager(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public bool IsApplicableForSourceGeneration(ClassDeclarationSyntax node)
    {
        if (!node.Modifiers.Any(SyntaxKind.PartialKeyword) || node.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return false;
        }

        if (!node.AttributeLists.Any())
        {
            return false;
        }

        const string serviceAttributeFullName = $"{CoreConstants.AttributesNamespace}.{CoreConstants.ServiceAttributeName}";

        foreach (var attributeList in node.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeName = attribute.Name.GetText().ToString();

                if (!CoreConstants.ServiceAttributeName.StartsWith(attributeName))
                {
                    continue;
                }

                var attributeSymbol = _semanticModel.GetSymbolInfo(attribute).Symbol!.ContainingType;

                if (serviceAttributeFullName == attributeSymbol.ToDisplayString())
                {
                    return true;
                }
            }
        }

        return false;
    }

    public (string FileName, SyntaxNode Node) GenerateService(ClassDeclarationSyntax syntax)
    {
        var (interfaceDeclaration, interfaceIdentifier) = InterfaceServiceFactory.CreateInterfaceForClass(syntax);

        var classDeclaration = ClassServiceFactory.GeneratePartialClassWithInterface(syntax, interfaceIdentifier);

        var namespaceName = _semanticModel
            .GetDeclaredSymbol(syntax)!
            .ContainingNamespace
            .ToDisplayString();

        var namespaceDeclaration = SyntaxNodesFactory.CreateNamespace(
            namespaceName,
            new MemberDeclarationSyntax[] { classDeclaration, interfaceDeclaration });

        var root = SyntaxFactory.CompilationUnit()
            .WithMembers(SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                namespaceDeclaration
            }))
            .WithUsings(SyntaxNodesFactory.CreateUsingDirectiveList(new[]
            {
                CoreConstants.AttributesNamespace
            }));

        return (syntax.Identifier.Text, root);
    }
}
