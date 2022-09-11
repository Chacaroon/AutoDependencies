using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
public partial class ThirdService
{
    public string GetString()
    {
        return "String form third service";
    }
}
