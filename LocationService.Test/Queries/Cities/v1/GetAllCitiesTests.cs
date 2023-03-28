using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Cities.Queries.v1;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
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
        var result = (List<CityData>)await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
