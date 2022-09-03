using AutoDependencies.Attributes;

namespace AutoDependencies.IntegrationTests.TestData;

[Service]
internal partial class TestService
{
    private readonly ITestDependency _depenency;

    public string GetDataFromDependency()
    {
        return _depenency.ReturnTestValue();
    }
}