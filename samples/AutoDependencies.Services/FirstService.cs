using System;
using System.Collections.Generic;
using AutoDependencies.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace AutoDependencies.Services;

[Service]
internal partial class FirstService
{
    private readonly HashSet<string> _cache;
    private readonly ISecondService _secondService;
    [Inject]
    public IThirdService ThirdService { get; }

    public IServiceProviderFactory<string>? DoSmth()
    {
        return null;
    }
}
