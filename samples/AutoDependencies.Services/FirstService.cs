using System.Collections.Generic;
using AutoDependencies.Attributes;
using AutoDependencies.Services.Interfaces.Generated;
using Microsoft.Extensions.DependencyInjection;

namespace AutoDependencies.Services;

[Service]
internal partial class FirstService
{
    private readonly HashSet<string>? _cache;
    private readonly ISecondService _secondService;
    [Inject]
    public IThirdService ThirdService { get; }

    public IServiceProviderFactory<string>? DoSmth(ISecondService secondService)
    {
        return null;
    }
}
