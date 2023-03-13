using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Cities.Commands.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Cities.v1;

public class DeleteCityTests
{
    [Fact]
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
