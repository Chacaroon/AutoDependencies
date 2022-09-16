namespace AutoDependencies.Generator.Models;

public record ConstructorInfo(
    ConstructorMemberInfo[] ConstructorMembers,
    ConstructorMemberInfo[] ExternalConstructorMembers)
{
    public bool HasExternalConstructor => ExternalConstructorMembers.Any();
}