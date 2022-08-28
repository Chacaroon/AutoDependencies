# AutoDependencies

AutoDependencies is a library designed to get rid of boilerplate code in ASP.NET services by generating it with Incremental Generator.

## Features

- [x] Generate a partial service
- [x] Generate constructor with injected services and save ones to the appropriate service members
- [x] Generate an interface with public methods of a service
- [ ] Generate method that registers all generated services to the `IServiceCollection`
- [ ] Generate code with Incremental Generatort
- [ ] Add code analyzer with code fix
- [ ] Cover generator and analyzer with tests
- [ ] Create and publish nuget packages

## Installation

TODO: Will be described after publishing packages to NuGet

## Usage

### ⚠️ The flow described below will be valid when the Generate Code with an Incremental Generator feature is implemented. For now, you can set up a [Playgroun](/docs/playground.md) to trigger code generation with console.

First, create the service with some dependencies and public methods. Mark just created service with `[Service]` attribute, so source generator will process it.

```csharp
// SampleService.cs
using AutoDependencies.Attributes;

namespace BestServiceEver;

[Service]
partial class SampleService
{
    private readonly IDependency _dependency;

    public void DoWork() 
    {
        _dependency.DoAnotherWork();
    }
}

// IDependency.cs
namespace BestServiceEver.Interfaces;

interface IDependency
{
    void DoAnotherWork();
}
```

Then build the project to make Incremental Generator generate the rest of code you need for the service. Once you do this, the Incremental Generator will generate the following code.

```csharp
using AutoDependencies.Attributes;

namespace BestServiceEver 
{
    [Generated]
    public partial class FirstService : IFirstService
    {
        public FirstService(BestServiceEver.Interfaces.IDependency dependency)
        {
            _dependency = dependency;
        }
    }

    [Generated]
    public interface IFirstService
    {
        void DoWork();
    }
}
```

## What's next

Check out the [Conventions and restricts](./docs/convention.md) guide to learn more about which parts of your services can be auto-generated.
