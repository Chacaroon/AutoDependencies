using AutoDependencies.Tests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencies.Generator;
using AutoDependencies.Tests.Helpers;

namespace AutoDependencies.Tests;

[UsesVerify]
public class ServiceInterfaceSnapshotTests : SnapshotTestsBase<ServiceGenerator>
{
    [Fact]
    public Task PublicMethod_Void_GeneratesInterfaceMember()
    {
        var source = GetSource("public void TestMethod() {}");

        return Verify(source);
    }

    [Fact]
    public Task PublicMethod_String_GeneratesInterfaceMember()
    {
        var members = @"
    public string TestMethod() {
        return null;
    }";
        
        var source = GetSource(members);

        return Verify(source);
    }

    [Fact]
    public Task PublicMethodWithExpressionBody_String_GeneratesInterfaceMember()
    {
        var members = @"
    public string TestMethod() => null;";
        
        var source = GetSource(members);

        return Verify(source);
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

        return Verify(source).UseParameters(modifier);
    }
}
