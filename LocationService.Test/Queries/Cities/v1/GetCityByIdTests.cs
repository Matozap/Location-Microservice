using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Cities.Queries;
using LocationService.Application.Cities.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.Cities.v1;

public class GetCityByIdTests
{
    [Fact]
    public async Task GetCityByIdTestsTest()
    {
        var classToHandle = new GetCityById
        {
            Id = CityMockBuilder.GenerateMockCity().Id
        };

        var handler = (GetCityByIdHandler)CityMockBuilder.CreateHandler<GetCityByIdHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void GetCityByClientIdInvalidRangeTest()
    {
        var classToHandle = new GetCityById();
        
        var handler = (GetCityByIdHandler)CityMockBuilder.CreateHandler<GetCityByIdHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
