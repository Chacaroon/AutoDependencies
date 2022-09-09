#nullable disable

using Microsoft.CodeAnalysis;

namespace AutoDependencies.Generator.Models;
public record ServiceInfo(SyntaxToken Name, SyntaxTokenList Modifiers, string Namespace)
{
    public string InterfaceName => $"I{Name}";
}
