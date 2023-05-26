using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Handlers.Cities.Commands;
using LocationService.Message.Cities;
using LocationService.Message.Cities.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Cities.v1;

public class UpdateCityTests
{
    [Fact]
    public async Task UpdateCityTest()
    {
        var classToHandle = new UpdateCity
        {
            Details = CityMockBuilder.GenerateMockCityDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateCityHandler)CityMockBuilder.CreateHandler<UpdateCityHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void UpdateCityInvalidCityIdTest()
    {
        var classToHandle = new UpdateCity
        {
            Details = new CityData()
        };

        var handler = (UpdateCityHandler)CityMockBuilder.CreateHandler<UpdateCityHandler>();

        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
