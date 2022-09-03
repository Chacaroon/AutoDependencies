using AutoDependencies.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencies.Generator;

namespace AutoDependencies.Tests;

[UsesVerify]
public class ServiceAttributeSnapshotTests : SnapshotTestsBase<ServiceGenerator>
{
    [Fact]
    public Task PartialClassWithServiceAttribute_GeneratesPartialClassWithEmptyConstructorAndInterface()
    {
        var source = @"
using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
internal partial class TestService {}";

        return Verify(source);
    }

    [Fact]
    public Task PartialClassWithoutServiceAttribute_GeneratesNothing()
    {
        var source = @"
using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

internal partial class TestService {}";

        return Verify(source);
    }

    [Fact]
    public Task ClassWithServiceAttribute_GeneratesNothing()
    {
        var source = @"
using AutoDependencies.Attributes;

namespace AutoDependencies.Services;

[Service]
internal class TestService {}";

        return Verify(source);
    }
}
