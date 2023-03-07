using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Commands.Cities.v1;
using LocationService.Message.Messaging.Request.Cities.v1;
using LocationService.Test.MockBuilder;
using LocationService.Test.Queries.Cities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.Cities.v1;

[TestClass]
public class DeleteCityTests
{
    [TestMethod]
    public async Task DeleteCityTest()
    {
        // Arrange
        var classToHandle = new DeleteCity
        {
            Id = CityMockBuilder.GenerateMockCity().Id
        };

        
        var handler = (DeleteCityHandler)CityMockBuilder.CreateHandler<DeleteCityHandler>();

        //Act
        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }
}
