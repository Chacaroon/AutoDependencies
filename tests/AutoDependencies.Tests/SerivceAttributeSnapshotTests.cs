using AutoDependencies.Generator;
using AutoDependencies.Tests.Helpers;

namespace AutoDependencies.Tests;

[UsesVerify]
public class AttributeSnapshotTests : SnapshotTestsBase<ServiceGenerator>
{
    [Fact]
    public Task PartialClassWithServiceAttribute_GeneratesPartialClassWithEmptyConstructorAndInterface()
    {
        var source = @"
using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
internal partial class TestService {}";

        return VerifyServiceAsync(source);
    }

    [Fact]
    public Task PartialClassWithoutServiceAttribute_GeneratesNothing()
    {
        var source = @"
using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

internal partial class TestService {}";

        return VerifyServiceAsync(source);
    }

    [Fact]
    public Task ClassWithServiceAttribute_GeneratesNothing()
    {
        var source = @"
using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
internal class TestService {}";

        return VerifyServiceAsync(source);
    }
}
