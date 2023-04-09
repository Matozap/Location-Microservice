using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Countries.Queries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.Countries.v1;

public class GetAllCountriesTests
{
    [Fact]
    public async Task GetAllCountriesTest()
    {
        var classToHandle = new GetAllCountries();
        
        var handler = (GetAllCountriesHandler)CountryMockBuilder.CreateHandler<GetAllCountriesHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
