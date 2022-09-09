using AutoDependencies.Attributes;

namespace AutoDependencies.IntegrationTests.TestData.External;

[Service]
internal partial class TestDependency
{
    public string ReturnTestValue()
    {
        return Constants.TestDataString;
    }
}