using AutoDependencies.Generator.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDependencies.Generator.Models;

public record ConstructorMemberInfo(string Name, TypeSyntax Type)
{
    public virtual bool Equals(ConstructorMemberInfo? constructorMemberInfo) => 
        Type.ToString() == constructorMemberInfo?.Type.ToString();

    public override int GetHashCode() => Type.ToString().GetHashCode();
};
