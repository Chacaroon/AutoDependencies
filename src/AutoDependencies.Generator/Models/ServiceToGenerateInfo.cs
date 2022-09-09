namespace AutoDependencies.Generator.Models;

public record ServiceToGenerateInfo(
    ServiceInfo ServiceInfo,
    InterfaceInfo InterfaceInfo,
    ConstructorMemberInfo[] ConstructorMembers,
    bool NullableEnabled);
