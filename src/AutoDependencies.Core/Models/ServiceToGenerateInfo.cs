namespace AutoDependencies.Core.Models;

public record ServiceToGenerateInfo(
    ServiceInfo ServiceInfo,
    InterfaceMemberInfo[] InterfaceMembers,
    ConstructorMemberInfo[] ConstructorMembers);
