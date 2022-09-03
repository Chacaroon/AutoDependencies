#nullable disable

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Models;
public class ConstructorMemberInfo
{
    public string Name { get; set; }

    public TypeSyntax Type { get; set; }
}
