using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencies.Generator;
using AutoDependencies.Tests.Data;
using AutoDependencies.Tests.Helpers;
using Microsoft.CodeAnalysis;

namespace AutoDependencies.Tests;
[UsesVerify]
public class NullableContextSnapshotTests : SnapshotTestsBase<ServiceGenerator>
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task PrivateReadonlyField_NullableContext_ShouldAddNullableDirectiveIfEnabled(bool nullableContextEnabled)
    {
        var usingDirectives = new[] { "using ExternalLib.Services;" };
        var members = "private readonly IExternalService? _externalService;";

        var source = GetSource(members, usingDirectives);
        var nullableContextOptions = nullableContextEnabled 
            ? NullableContextOptions.Enable 
            : NullableContextOptions.Disable;

        return VerifyServiceAsync(source, new[] { TestData.ExternalService }, nullableContextOptions)
            .UseParameters(nullableContextOptions);
    }
}
