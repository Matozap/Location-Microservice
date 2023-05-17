using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Handlers.Countries.v1.Queries;
using LocationService.Message.Contracts.Countries.v1.Requests;
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
