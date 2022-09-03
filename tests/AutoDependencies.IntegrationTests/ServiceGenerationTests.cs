using AutoDependencies.IntegrationTests.TestData;
using Microsoft.Extensions.DependencyInjection;

namespace AutoDependencies.IntegrationTests;

public class ServiceGenerationTests
{
    [Fact]
    public void UseGeneratedInterfacesAndMethods_DependencyReturnsCorrectData()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ITestService, TestService>();
        serviceCollection.AddTransient<ITestDependency, TestDependency>();
        using var serviceProvider = serviceCollection.BuildServiceProvider();

        // Act
        var service = serviceProvider.GetService<ITestService>()!;
        var testData = service.GetDataFromDependency();

        // Assert
        Assert.Equal(Constants.TestDataString, testData);
    }
}
