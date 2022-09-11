using AutoDependencies.Attributes;

namespace AutoDependencies.Services.External;

[Service]
public partial class ExternalService
{
    public string GetString()
    {
        return "String from external service";
    }
}