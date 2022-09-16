using AutoDependencies.Services.Extensions.Generated;
using AutoDependencies.Services.Interfaces.Generated;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection()
    .RegisterServicesFormAutoDependenciesServices(ServiceLifetime.Scoped);

var provider = serviceCollection.BuildServiceProvider();

var firstService = provider.GetRequiredService<IFirstService>();
        
firstService.ConsoleDataFromInjectedServices();