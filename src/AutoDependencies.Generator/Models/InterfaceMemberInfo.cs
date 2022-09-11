using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Models;
public record InterfaceMemberInfo(string Name, ParameterListSyntax ParameterList, TypeSyntax ReturnType);