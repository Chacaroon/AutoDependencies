using AutoDependencies.Services;
using Microsoft.Extensions.DependencyInjection;

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

