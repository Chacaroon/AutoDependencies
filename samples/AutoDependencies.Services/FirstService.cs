using AutoDependencies.Attributes;
using AutoDependencies.Services.External.Interfaces.Generated;
using AutoDependencies.Services.Interfaces.Generated;
using System;

namespace AutoDependencies.Services;

[Service]
internal partial class FirstService
{
    private readonly ISecondService _secondService;
    private readonly IExternalService _externalService;

    [ServiceConstructor]
    private FirstService(IThirdService thirdService) : this(thirdService.GetString())
    {
        Console.WriteLine($"Constructor: {thirdService.GetString()}");
    }
    
    private FirstService(string thirdService)
    {
        Console.WriteLine($"Another constructor: {thirdService}");
    }
    
    public void ConsoleDataFromInjectedServices()
    {
        Console.WriteLine(_secondService.GetString());
        Console.WriteLine(_externalService.GetString());
    }
}
