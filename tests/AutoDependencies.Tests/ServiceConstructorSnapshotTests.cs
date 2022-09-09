using AutoDependencies.Generator;
using AutoDependencies.Generator.Constants;
using AutoDependencies.Tests.Data;
using AutoDependencies.Tests.Helpers;

namespace AutoDependencies.Tests;

[UsesVerify]
public class ServiceConstructorSnapshotTests : SnapshotTestsBase<ServiceGenerator>
{
    [Fact]
    public Task PrivateReadonlyField_GeneratesConstructorWithOneParameterAndAssignProperty()
    {
        var usingDirectives = new[] { "using ExternalLib.Services;" };
        var members = "private readonly IExternalService _externalService;";

        var source = GetSource(members, usingDirectives);

        return Verify(source, new[] { TestData.ExternalService });
    }

    [Theory]
    [InlineData("IExternalService?")]
    [InlineData("List<IExternalService>?")]
    [InlineData("List<IExternalService?>")]
    [InlineData("List<IExternalService?>?")]
    [InlineData("int?")]
    [InlineData("int[]?")]
    [InlineData("int?[]")]
    [InlineData("int?[]?")]
    [InlineData("Nullable<int>")]
    [InlineData("Nullable<int[]>")]
    public Task PrivateReadonlyFieldNullableType_GeneratesConstructorAndAssignProperty(string type)
    {
        var usingDirectives = new[]
        {
            "using System;",
            "using System.Collections.Generic;",
            "using ExternalLib.Services;"
        };
        var members = $"private readonly {type} _externalService;";

        var source = GetSource(members, usingDirectives);

        return Verify(source, new[] { TestData.ExternalService }).UseParameters(type.Replace("?", "null"));
    }

    [Fact]
    public Task PropertiesAndFieldsWithInjectAttribute_GeneratesConstructorAndAssignPropertiesAndFields()
    {
        var usingDirectives = new[] { "using ExternalLib.Services;" };
        var members = @"
    [Inject]
    internal readonly IExternalService _externalService1;
    [Inject]
    public IExternalService ExternalService2 { get; };
    [Inject]
    protected IExternalService ExternalService3 { get; set; };";

        var source = GetSource(members, usingDirectives);

        return Verify(source, new[] { TestData.ExternalService });
    }

    [Fact]
    public Task PropertiesAndFieldsWithoutInjectAttribute_GeneratesConstructorWithNoParameters()
    {
        var usingDirectives = new[] { "using ExternalLib.Services;" };
        var members = @"
    internal readonly IExternalService _externalService1;
    public IExternalService ExternalService2 { get; };
    protected IExternalService ExternalService3 { get; set; };";

        var source = GetSource(members, usingDirectives);

        return Verify(source, new[] { TestData.ExternalService });
    }

    [Fact]
    public Task InjectGeneratedServiceInterface_ResolvesGeneratedInterface()
    {
        var usingDirectives = new[] { $"using {GeneratorConstants.PredefinedNamespaces.GeneratedInterfacesNamespace};" };
        var members = "private readonly ISecondService _secondService;";

        var source = GetSource(members, usingDirectives);

        return Verify(source, new[] { TestData.SecondServiceWithServiceAttribute });
    }
}