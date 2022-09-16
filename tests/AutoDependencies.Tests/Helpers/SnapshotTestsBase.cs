using Microsoft.CodeAnalysis;

namespace AutoDependencies.Tests.Helpers;
public abstract class SnapshotTestsBase<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
    protected virtual string ServiceTemplate => @"
using AutoDependencies.Attributes;
{using}

namespace AutoDependencies.Services;

[Service]
internal partial class TestService {
    {members}
}";

    protected SettingsTask VerifyServiceAsync(string source, string[]? additionalSources = null, NullableContextOptions nullableContextOptions = NullableContextOptions.Disable)
    {
        var (output, diagnostics, _) = TestHelper.GetGeneratedOutput<TGenerator>(source, additionalSources, nullableContextOptions);

        Assert.Empty(diagnostics);

        return Verify(output).UseDirectory("../Snapshots/Services");
    }

    protected SettingsTask VerifyExtensions(string source, string[]? additionalSources = null, NullableContextOptions nullableContextOptions = NullableContextOptions.Disable)
    {
        var (_, diagnostics, extensions) = TestHelper.GetGeneratedOutput<TGenerator>(source, additionalSources, nullableContextOptions);

        Assert.Empty(diagnostics);

        return Verify(extensions).UseDirectory("../Snapshots/Extensions");
    }

    protected string GetSource(string serviceMembers = "", string[]? usingDirectives = null)
    {
        var usingSource = string.Join("\r\n", usingDirectives ?? Array.Empty<string>());

        return ServiceTemplate
            .Replace("{using}", usingSource)
            .Replace("{members}", serviceMembers);
    }
}
