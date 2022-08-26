using System.Collections.Generic;
using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
public partial class FirstService
{
    private readonly ISecondService _secondService;

    [Inject]
    public IThirdService ThirdService { get; set; }

    private readonly Dictionary<string, string> _cache = new();
    private readonly Dictionary<string, string> _cache1;

    public void DoSmth()
    {
        _secondService.DoSmthElse();
    }
}

