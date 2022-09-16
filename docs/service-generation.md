## Service generation
In order to start using code generation, you must add the `partial` keyword and the `[Service]` attribute to a service. 
The `[Service]` attribute tells the source code generator that this service is applicable for extension with the generated code.
The `partial` keyword is required so that the service can be extended.

```csharp
using AutoDependencies.Attributes;

namespace SampleProject.Services;

[Service]
public partial class SampleService
{
    // ...
}
```

In this case the following code will be generated for the service:

```csharp
using AutoDependencies.Attributes;
using SampleProject.Interfaces.Generated;

// This annotation will be added if the nullable reference types are enabled
#nullable enable 
namespace SampleProject.Services
{
    [Generated]
    public partial class SampleService : ISampleService
    {
        public SampleService()
        {
        }
    }
}

// Interfaces are placed in the <AssemblyName>.Interfaces.Generated namespace
namespace SampleProject.Interfaces.Generated
{
    [Generated]
    public interface ISampleService
    {
    }
}
```

In addition to the above, one more class will be generated. This class will contain an extension method for the [IServiceCollection](https://docs.microsoft.com/ru-ru/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-6.0) that will register all generated services in the dependency injection container.

```csharp
using SampleProject.Services;
using AutoDependencies.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SampleProject.Interfaces.Generated;

namespace SampleProject.Extensions.Generated
{
    [Generated]
    public static class SampleProjectServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServicesFormSampleProject(this IServiceCollection services, ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(typeof(ISampleService), typeof(SampleService), lifetime));
            return services;
        }
    }
}
```