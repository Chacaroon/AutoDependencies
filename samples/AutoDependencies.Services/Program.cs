using Microsoft.Extensions.DependencyInjection;

namespace AutoDependencies.Services;

class Program
{
    public static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IFirstService, FirstService>();
        
        var a = new FirstService(null, new SecondService(), new ThirdService());

        var b = a.DoSmth();
    }
}