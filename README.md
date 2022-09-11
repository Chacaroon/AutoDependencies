# AutoDependencies

[![NuGet](https://img.shields.io/nuget/v/AutoDependencies.svg)](https://www.nuget.org/packages/AutoDependencies/)

AutoDependencies is a library designed to get rid of boilerplate code in ASP.NET services by generating it with Incremental Generator.

## Features

- [x] Generate a partial service
- [x] Generate constructor with injected services and save ones to the appropriate service members
- [x] Generate an interface with public methods of a service
- [x] Generate code with Incremental Generatort
- [x] Create and publish nuget packages
- [x] Cover generator with tests 
- [x] Generate method that registers all generated services to the `IServiceCollection`
- [ ] Add code analyzer with code fix
- [ ] Cover analyzer with tests
- [ ] Generate an interface for the service depending on a flag (i.e. supress interface generation)
- [ ] Support for a custom constructor in addition to the generated one

## Installation

Add the package to your application using

```shell
Install-Package AutoDependencies.Generator
```


This adds a `<PackageReference>` to your project. You can additionally mark the package as `PrivateAssets="all"`.

> Setting `PrivateAssets="all"` means any projects referencing this one won't get a reference to the _AutoDependencies.Generator_ package.
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

  <!-- Add the package -->
  <PackageReference Include="AutoDependencies.Generator" Version="..." PrivateAssets="all"/>

</Project>
```

Adding the package will automatically add a marker attributes, such as `[Service]` and `[Inject]`, to your project.

## Usage

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
    public partial class SampleService : ISampleService
    {
        public SampleService(BestServiceEver.Interfaces.IDependency dependency)
        {
            _dependency = dependency;
        }
    }
}

namespace BestServiceEver.Interfaces.Generated
{
    [Generated]
    public interface ISampleService
    {
        void DoWork();
    }
}
```

In addition to the above, one more class will be generated. This class will contain an extension method for the [IServiceCollection](https://docs.microsoft.com/ru-ru/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-6.0) that will register all generated services in the dependency injection container.

```csharp
using BestServiceEver;
using AutoDependencies.Attributes;
using Microsoft.Extensions.DependencyInjection;
using BestServiceEver.Interfaces.Generated;

namespace BestServiceEver.Extensions.Generated
{
    [Generated]
    public static class BestServiceEverServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServicesFormBestServiceEver(this IServiceCollection services, ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(typeof(IFirstService), typeof(FirstService), lifetime));
            return services;
        }
    }
}
```

## What's next

Check out the [Conventions and restricts](./docs/convention.md) guide to learn more about which parts of your services can be auto-generated.
