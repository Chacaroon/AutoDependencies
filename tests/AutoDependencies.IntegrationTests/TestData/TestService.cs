using AutoDependencies.Attributes;
using AutoDependencies.IntegrationTests.TestData.External;
#if NUGET_INTEGRATION_TESTS
using AutoDependencies.NugetIntegrationTests.Interfaces.Generated;
#else
using AutoDependencies.IntegrationTests.Interfaces.Generated;
#endif

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