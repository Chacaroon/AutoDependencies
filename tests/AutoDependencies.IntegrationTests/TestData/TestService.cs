using AutoDependencies.Attributes;
using AutoDependencies.Interfaces.Generated;

namespace AutoDependencies.IntegrationTests.TestData;

[Service]
internal partial class TestService
{
    private readonly ITestDependency _dependency;

    public string GetDataFromDependency()
    {
        return _dependency.ReturnTestValue();
    }
}