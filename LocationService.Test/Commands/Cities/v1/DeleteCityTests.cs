using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Cities.Commands;
using LocationService.Application.Cities.Requests;
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
