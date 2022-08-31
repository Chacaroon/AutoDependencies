using Microsoft.CodeAnalysis;

#nullable disable

namespace AutoDependencies.Core.Models;
public class ServiceInfo
{
    public SyntaxToken Name { get; set; }
    
    public SyntaxTokenList Modifiers { get; set; }

    public string Namespace { get; set; }

    public string InterfaceName => $"I{Name}";
}
