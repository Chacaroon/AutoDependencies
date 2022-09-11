using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Models;

public record ConstructorMemberInfo(string Name, TypeSyntax Type);
