using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Cities.Commands.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Cities.v1;

public class SoftDeleteCityTests
{
    [Fact]
    public async Task SoftDeleteCityTest()
    {
        var classToHandle = new SoftDeleteCity
        {
            Id = CityMockBuilder.GenerateMockCity().Id
        };
        
        var handler = (SoftDeleteCityHandler)CityMockBuilder.CreateHandler<SoftDeleteCityHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }
}
