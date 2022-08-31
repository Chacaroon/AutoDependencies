using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable disable

namespace AutoDependencies.Core.Models;
public class InterfaceMemberInfo
{
    public string Name { get; set; }

    public TypeSyntax ReturnType { get; set; }

    public ParameterListSyntax ParameterList { get; set; }
}
