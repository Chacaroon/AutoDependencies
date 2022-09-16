# AutoDependencies

[![BuildAndPublish](https://github.com/Chacaroon/AutoDependencies/actions/workflows/BuildAndPublish.yml/badge.svg)](https://github.com/Chacaroon/AutoDependencies/actions/workflows/BuildAndPublish.yml)
[![NuGet](https://img.shields.io/nuget/v/AutoDependencies.Generator.svg)](https://www.nuget.org/packages/AutoDependencies.Generator/)

AutoDependencies is a library designed to get rid of boilerplate code in ASP.NET services by generating it with Incremental Generator.

## Features

- [x] Generate a partial service
- [x] Generate constructor with injected services and save ones to the appropriate service members
- [x] Generate an interface with public methods of a service
- [x] Generate code with Incremental Generator
- [x] Create and publish nuget packages
- [x] Cover generator with tests 
- [x] Generate method that registers all generated services to the `IServiceCollection`
- [x] Support for a custom constructor in addition to the generated one
- [ ] Add code analyzer with code fix
- [ ] Generate an interface for the service depending on a flag (i.e. supress interface generation)

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
using AutoDependencies.Attributes;

namespace Services;

[Service]
partial class SampleService
{
    private readonly IDependency _dependency;

    public void DoWork() 
    {
        _dependency.DoAnotherWork();
    }
}
```

Once you do this, the incremental generator will generate:
- an interface based on public methods of the service;
- a constructor that injects and initializes dependencies based on private readonly properties;
- extension method for [IServiceCollection](https://docs.microsoft.com/ru-ru/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-6.0) that registers the generated services;

## What's next
Check out more detailed guides on the various features:  
[🛠️ Service generation 🛠️](./docs/service-generation.md)  
[📜 Interface generation 📜](./docs/interface-generation.md)  
[🔗 Injecting dependencies 🔗](./docs/injecting-dependencies.md)  
[🧰 Custom constructor 🧰](./docs/custom-constructor.md)  
[💥 Service collection extension 💥](./docs/service-collection-extension.md)