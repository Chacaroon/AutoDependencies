#nullable disable

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Models;
public class InterfaceMemberInfo
{
    public string Name { get; set; }

    public TypeSyntax ReturnType { get; set; }

    public ParameterListSyntax ParameterList { get; set; }
}
