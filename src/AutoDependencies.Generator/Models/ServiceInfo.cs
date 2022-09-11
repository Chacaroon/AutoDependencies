using Microsoft.CodeAnalysis;

namespace AutoDependencies.Generator.Models;
public record ServiceInfo(SyntaxToken ServiceName, SyntaxTokenList Modifiers, string NamespaceName);
