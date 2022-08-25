using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
public partial class FirstService
{
    [Inject]
    private readonly ISecondService _secondService;

    public void DoSmth()
    {
        _secondService.DoSmthElse();
    }
}

