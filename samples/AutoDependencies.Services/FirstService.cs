using AutoDependencies.Attributes;
using AutoDependencies.Services.Interfaces.Generated;
using System;

namespace AutoDependencies.Services;

[Service]
internal partial class FirstService
{
    private readonly ISecondService _secondService = default!;

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
    }
}
