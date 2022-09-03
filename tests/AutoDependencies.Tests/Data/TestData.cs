using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencies.Tests.Data;
internal static class TestData
{
    public static string ExternalService = @"
namespace ExternalLib.Services {
    public interface IExternalService {}
}";
}
