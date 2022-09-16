## Injecting dependencies
In order to inject dependency, you must write at least three lines of code

```csharp
using ExternalLib.Services;

namespace SampleProject;

class SampleService.Services; 
{
    // first
    private readonly IDependency _dependency;
    
    // second
    public SampleService(IDependency dependency)
    {
        // third
        _dependency = dependency;
    }
}
```

But you really only need one line that will allow you to access the injected dependency. Let the source generator take care of the other ones.  

Let's take a look at this service and output will be generated.
```csharp
using AutoDependencies.Attributes;
using ExternalLib.Services;

namespace SampleProject.Services;

[Service]
public partial class SampleService
{
    private readonly IDependency _dependency;
}
```
The source generator is looking for private readonly fields in a service and injects dependencies to ones automatically.  
The following code will be generated for this service:
```csharp
using AutoDependencies.Attributes;
using SampleProject.Interfaces.Generated;

#nullable enable
namespace SampleProject.Services
{
    [Generated]
    public partial class SampleService : ISampleService
    {
        public SampleService(ExternalLib.Services.IDependency dependency)
        {
            _dependency = dependency;
        }
    }
}

// Generated interface...
```


But what if you need to inject a dependency into a field or property other than a private readonly one? In this case, you can explicitly specify that the service generator must process your field or property.

For this purpose, you can use the `[Inject]` attribute.
```csharp
using AutoDependencies.Attributes;
using ExternalLib.Services;

namespace SampleProject.Services;

[Service]
public partial class SampleService
{
    [Inject] // Force the source generator to inject that dependency. 
    public IDependency Dependency { get; };
}
```

In this case, the dependency will be injected in the same way as in the example above.