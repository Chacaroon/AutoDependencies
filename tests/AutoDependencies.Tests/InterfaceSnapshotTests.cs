using AutoDependencies.Generator;
using AutoDependencies.Tests.Data;
using AutoDependencies.Tests.Helpers;

namespace AutoDependencies.Tests;

[UsesVerify]
public class InterfaceSnapshotTests : SnapshotTestsBase<ServiceGenerator>
{
    [Fact]
    public Task PublicMethod_Void_GeneratesInterfaceMember()
    {
        var source = GetSource("public void TestMethod() {}");

        return VerifyServiceAsync(source);
    }

    [Fact]
    public Task PublicMethod_String_GeneratesInterfaceMember()
    {
        var members = @"
    public string TestMethod() {
        return null;
    }";

        var source = GetSource(members);

        return VerifyServiceAsync(source);
    }

    [Fact]
    public Task PublicMethodWithExpressionBody_String_GeneratesInterfaceMember()
    {
        var members = @"
    public string TestMethod() => null;";

        var source = GetSource(members);

        return VerifyServiceAsync(source);
    }

    [Theory]
    [InlineData("private")]
    [InlineData("protected")]
    [InlineData("internal")]
    [InlineData("")]
    public Task InaccessibleMethod_Void_DoesNotGenerateInterfaceMember(string modifier)
    {
        var members = @$"
    {modifier} void TestMethod() {{
        return;
    }}";

        var source = GetSource(members);

        return VerifyServiceAsync(source).UseParameters(modifier);
    }


    [Fact]
    public Task PublicMethodWithPrimitiveTypeParameter_Void_GeneratesInterfaceMemberWithParameters()
    {
        var members = @"
    public void TestMethod(int parameter) {}";

        var source = GetSource(members);

        return VerifyServiceAsync(source);
    }

    [Fact]
    public Task PublicMethodWithExternalTypeParameter_Void_GeneratesInterfaceMemberWithParameters()
    {
        var usingDirectives = new[] { "using ExternalLib.Services;" };
        var members = @"
    public void TestMethod(IExternalService parameter) {}";

        var source = GetSource(members, usingDirectives);

        return VerifyServiceAsync(source, new[] { TestData.ExternalService });
    }
}
