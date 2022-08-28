# Conventions and restricts
This document describes conventions you must follow to make source generator catch and process your service and it's members.

## Conventions
### Service

In order to make service applicable for processing for source generator, it must meet the following reqirements:
1. To have `[Service]` attribute.
2. To have `partial` modifier.

Valid service example:
```csharp
// SomeService.cs
using AutoDependencies.Attributes;

namespace SomeNamespace;

[Service]
partial class SomeService { }
```

For this service will be generated the following code:
```csharp
// SomeService.g.cs
using AutoDependencies.Attributes;

namespace SomeNamespace 
{
    [Generated]
    partial class SomeService : ISomeService 
    {
        public SomeService()
        {
        }
    }
    
    [Generated]
    public interface ISomeService
    {
    }
}
```

### Injected dependencies

There are several ways to inject dependency.

#### private readonly fields

By default, source generator looks for all **unassigned** private readonly fields and injects dependencies to them.

Valid dependencies example:
```csharp
// SomeService.cs
using AutoDependencies.Attributes;

namespace SomeNamespace;

[Service]
partial class SomeService {
    // This dependency will be injected automatically
    private readonly IAnotherService _anotherService;
}
```

For this service will be generated the following code:
```csharp
// SomeService.g.cs
using AutoDependencies.Attributes;

namespace SomeNamespace 
{
    [Generated]
    partial class SomeService : ISomeService 
    {
        public SomeService(IAnotherService anotherService)
        {
            _anotherService = anotherService;
        }
    }
    
    ...
}
```

#### Any fields or properties

One day, you will need to inject a dependency on a property or field other than private readonly. In this case, you can simply mark your dependency member with the `[Inject]` attribute so that the source code generator will catch and process it.

Valid dependencies example:
```csharp
// SomeService.cs
using AutoDependencies.Attributes;

namespace SomeNamespace;

[Service]
partial class SomeService {
    // This dependency will be injected automatically
    [Inject]
    public IAnotherService AnotherService { get; set; };
}
```

For this service will be generated the following code:
```csharp
// SomeService.g.cs
using AutoDependencies.Attributes;

namespace SomeNamespace 
{
    [Generated]
    partial class SomeService : ISomeService 
    {
        public SomeService(IAnotherService anotherService)
        {
            AnotherService = anotherService;
        }
    }
    
    ...
}
```

### Interface

Interface will be generated for all the public methods in the service.

Valid service with public method:
```csharp
// SomeService.cs
using AutoDependencies.Attributes;

[Service]
partial class SomeService {
    public void DoSomething() 
    {
    }
}
```

For this service will be generated the following code:
```csharp
// SomeService.g.cs
using AutoDependencies.Attributes;

namespace SomeNamespace 
{
    [Generated]
    partial class SomeService : ISomeService 
    {
    }
    
    [Generated]
    public interface ISomeService
    {
        void DoSomething();
    }
}
```

## Restricts

The source code generator will not process static members.
