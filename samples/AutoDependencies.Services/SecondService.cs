using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
internal partial class SecondService
{
    public string GetString()
    {
        return "String form second service";
    }
}
