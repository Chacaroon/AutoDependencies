using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencies.Generator;
using AutoDependencies.Tests.Helpers;

namespace AutoDependencies.Tests;

[UsesVerify]
public class ServiceCollectionExtensionsSnapshotTests : SnapshotTestsBase<ServiceGenerator>
{
    [Fact]
    public Task ValidService_GeneratesServiceRegistration()
    {
        var source = GetSource();

        return VerifyExtensions(source);
    }

    [Fact]
    public Task NoServicesToGenerate_DoesNotGenerateServiceRegistration()
    {
        return VerifyExtensions(string.Empty);
    }
}
