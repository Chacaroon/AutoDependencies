using AutoDependencies.Attributes;

namespace AutoDependencies.IntegrationTests.TestData;

[Service]
internal partial class TestDependency
{
    public string ReturnTestValue()
    {
        return Constants.TestDataString;
    }
}