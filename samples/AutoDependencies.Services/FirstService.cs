using AutoDependencies.Attributes;
using AutoDependencies.Services.External.Interfaces.Generated;
using AutoDependencies.Services.Interfaces.Generated;
using System;

namespace AutoDependencies.Services;

[Service]
internal partial class FirstService
{
    private readonly ISecondService _secondService;
    [Inject]
    public IThirdService ThirdService { get; }

    private readonly IExternalService _externalService;

    public void ConsoleDataFromInjectedServices()
    {
        Console.WriteLine(_secondService.GetString());
        Console.WriteLine(ThirdService.GetString());
        Console.WriteLine(_externalService.GetString());
    }
}
