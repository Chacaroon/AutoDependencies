#nullable disable

using Microsoft.CodeAnalysis;

namespace AutoDependencies.Generator.Models;
public record ServiceInfo(SyntaxToken ServiceName, SyntaxTokenList Modifiers, string NamespaceName)
{
    public string InterfaceName => $"I{ServiceName}";
}
