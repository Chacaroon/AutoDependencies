using AutoDependencies.Generator;
using AutoDependencies.Tests.Data;
using AutoDependencies.Tests.Helpers;

namespace AutoDependencies.Tests;

[UsesVerify]
public class ExternalConstructorSnapshotTests : SnapshotTestsBase<ServiceGenerator>
{
    [Fact]
    public Task PrivateConstructorWithDependency_InjectDependencyAndPassToConstructor()
    {
        var usingDirectives = new[]
        {
            "using AutoDependencies.Attributes;",
            "using ExternalLib.Services;"
        };

        var members = @"
    private TestService(IExternalService externalService) {}";

        var source = GetSource(members, usingDirectives);

        return VerifyServiceAsync(source, new[] { TestData.ExternalService });
    }

    [Fact]
    public Task PrivateConstructorAndPrivateReadonlyFieldWithSameType_InjectOneDependency()
    {
        var usingDirectives = new[]
        {
            "using AutoDependencies.Attributes;",
            "using ExternalLib.Services;"
        };

        var members = @"
    private readonly IExternalService _externalService;

    private TestService(IExternalService externalService) {}";

        var source = GetSource(members, usingDirectives);

        return VerifyServiceAsync(source, new[] { TestData.ExternalService });
    }

    [Fact]
    public Task PrivateConstructorAndPrivateReadonlyFieldWithDifferentTypes_InjectTwoDependencies()
    {
        var usingDirectives = new[]
        {
            "using AutoDependencies.Attributes;",
            "using ExternalLib.Services;",
            "using AutoDependencies.Services.External;"
        };

        var members = @"
    private readonly ISecondService _externalService;

    private TestService(IExternalService externalService) {}";

        var source = GetSource(members, usingDirectives);

        return VerifyServiceAsync(source, new[] { TestData.ExternalService });
    }
    
    [Fact]
    public Task TwoPrivateConstructors_ExternalConstructorCallDoesNotGenerated()
    {
        var usingDirectives = new[]
        {
            "using AutoDependencies.Attributes;",
            "using ExternalLib.Services;"
        };

        var members = @"
    private TestService(IExternalService externalService) {}

    private TestService(string str) {}";

        var source = GetSource(members, usingDirectives);

        return VerifyServiceAsync(source, new[] { TestData.ExternalService });
    }
    
    [Fact]
    public Task TwoPrivateConstructorsAndServiceConstructorAttribute_ExternalConstructorCallGenerated()
    {
        var usingDirectives = new[]
        {
            "using AutoDependencies.Attributes;",
            "using ExternalLib.Services;"
        };

        var members = @"
    [ServiceConstructor]
    private TestService(IExternalService externalService) {}

    private TestService(string str) {}";

        var source = GetSource(members, usingDirectives);

        return VerifyServiceAsync(source, new[] { TestData.ExternalService });
    }
}