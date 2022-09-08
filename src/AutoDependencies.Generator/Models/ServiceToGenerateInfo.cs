namespace AutoDependencies.Generator.Models;

public record ServiceToGenerateInfo(
    ServiceInfo ServiceInfo,
    InterfaceMemberInfo[] InterfaceMembers,
    ConstructorMemberInfo[] ConstructorMembers,
    bool nullableEnabled);
