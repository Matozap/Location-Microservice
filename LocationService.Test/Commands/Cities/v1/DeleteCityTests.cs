using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Handlers.Cities.v1.Commands;
using LocationService.Message.Contracts.Cities.v1.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Cities.v1;

public class DeleteCityTests
{
    [Fact]
    public async Task DeleteCityTest()
    {
        var classToHandle = new DeleteCity
        {
            Id = CityMockBuilder.GenerateMockCity().Id
        };

        var handler = (DeleteCityHandler)CityMockBuilder.CreateHandler<DeleteCityHandler>();

        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }
}
