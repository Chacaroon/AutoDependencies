## Custom constructor
Sometimes it is necessary to define some logic other than dependency injection in the constructor. For this case AutoDependencies allows you to define your own constructor with a custom logic.

If you do so, your constructor call will be generated. In addition, all parameters will be recognized as dependencies and will be automatically injected to the constructor.

Let's take a look at the following service.

```csharp
using AutoDependencies.Attributes;
using SampleProject.Interfaces.Generated;
using ExternalLib.Services;

namespace SampleProject.Services;

[Service]
public partial class SampleService
{
    private readonly IAnotherDependency _anotherDependency;
 
    // The custom constructor must be non-public   
    private SampleService(IDependency dependency)
    {
        // Do stuff...
    }
}
```

A constructor will be generated for this service that injects dependencies for private readonly fields and custom constructor parameters. 

```csharp
using AutoDependencies.Attributes;
using SampleProject.Interfaces.Generated;

#nullable enable
namespace SampleProject.Services
{
    [Generated]
    public partial class SampleService : ISampleService
    {
        public SampleService(IAnotherDependency anotherDependency, IDependency dependency)
            : this(dependency)
        {
            _anotherDependency = anotherDependency;
        }
    }
}

// Generated interface...
```

If you have more than one constructor, you can specify which one should be used by the source code generator.

For this purpose, you can use the `[ServiceConstructor]` attribute.

```csharp
using AutoDependencies.Attributes;
using SampleProject.Interfaces.Generated;
using ExternalLib.Services;

namespace SampleProject.Services;

[Service]
public partial class SampleService
{
    private readonly IAnotherDependency _anotherDependency;
 
    // The source code generator will generate a call to this constructor.
    [ServiceConstructor]
    private SampleService(IDependency dependency)
    {
        // Do stuff...
    }
    
    private SampleService() {}
}
```