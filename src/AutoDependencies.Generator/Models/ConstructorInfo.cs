namespace AutoDependencies.Generator.Models;

public record ConstructorInfo(
    ConstructorMemberInfo[] ConstructorMembers,
    ConstructorMemberInfo[] CustomConstructorMembers)
{
    public bool HasExternalConstructor => CustomConstructorMembers.Any();
}