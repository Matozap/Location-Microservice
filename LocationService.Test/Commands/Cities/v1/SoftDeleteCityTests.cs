using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Cities.Commands.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using LocationService.Test.MockBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.Cities.v1;

[TestClass]
public class SoftDeleteCityTests
{
    [TestMethod]
    public async Task SoftDeleteCityTest()
    {
        // Arrange
        var classToHandle = new SoftDeleteCity
        {
            Id = CityMockBuilder.GenerateMockCity().Id
        };
        
        var handler = (SoftDeleteCityHandler)CityMockBuilder.CreateHandler<SoftDeleteCityHandler>();

        //Act
        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }
}
