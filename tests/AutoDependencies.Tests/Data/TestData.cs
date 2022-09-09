namespace AutoDependencies.Tests.Data;
internal static class TestData
{
    public static string ExternalService = @"
namespace ExternalLib.Services {
    public interface IExternalService {}
}";

    public static string SecondServiceWithServiceAttribute = @"
using AutoDependencies.Attributes;

namespace AutoDependencies.Services.External {
    [Service]
    public class SecondService {}
}";

}
