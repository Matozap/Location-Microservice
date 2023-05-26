using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Handlers.Cities.Queries;
using LocationService.Message.Cities.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.Cities.v1;

public class GetAllCitiesTests
{
    [Fact]
    public async Task GetAllCitiesTest()
    {
        var classToHandle = new GetAllCities
        {
            StateId = "1"
        };
        
        var handler = (GetAllCitiesHandler)CityMockBuilder.CreateHandler<GetAllCitiesHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
