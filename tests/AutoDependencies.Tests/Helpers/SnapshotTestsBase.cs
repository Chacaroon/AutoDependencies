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

    protected SettingsTask Verify(string source, string[]? additionalSources = null, NullableContextOptions nullableContextOptions = NullableContextOptions.Disable)
    {
        var (output, diagnostics) = TestHelper.GetGeneratedOutput<TGenerator>(source, additionalSources, nullableContextOptions);

        Assert.Empty(diagnostics);

        return Verifier.Verify(output).UseDirectory("../Snapshots");
    }

    protected string GetSource(string serviceMembers, string[]? usingDirectives = null)
    {
        var usingSource = string.Join("\r\n", usingDirectives ?? Array.Empty<string>());

        return ServiceTemplate
            .Replace("{using}", usingSource)
            .Replace("{members}", serviceMembers);
    }
}
