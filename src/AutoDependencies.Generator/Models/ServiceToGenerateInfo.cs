namespace AutoDependencies.Generator.Models;

public record ServiceToGenerateInfo(
    ServiceInfo ServiceInfo,
    InterfaceInfo InterfaceInfo,
    ConstructorInfo ConstructorInfo,
    bool NullableEnabled);
