## Interface generation
Once your service meets all the requirements to be processed by the source code generator, an interface for that service will be generated.  
Let's take a look at sample service.

```csharp
using AutoDependencies.Attributes;
using SampleProject.Models;

namespace SampleProject.Services;

[Service]
public partial class SampleService
{
    public string Process(Model model)
    {
        return model.SomeData;
    }
    
    public static string ProcessStatic(Model model)
    {
        return model.SomeData;
    }
    
    private string ProcessPrivate(Model model)
    {
        return model.SomeData;
    }
}
```

The following code will be generated for this service:

```csharp
using AutoDependencies.Attributes;
using SampleService.Interfaces.Generated;

// Generated service...

namespace SampleProject.Interfaces.Generated
{
    [Generated]
    public interface ISampleService
    {
        string Process(SampleProject.Models.Model model);
    }
}
```