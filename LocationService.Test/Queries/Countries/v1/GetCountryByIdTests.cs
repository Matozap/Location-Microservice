using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Handlers.Countries.Queries;
using LocationService.Message.Countries.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.Countries.v1;

public class GetCountryByIdTests
{
    [Fact]
    public async Task GetCountryByIdTestsTest()
    {
        var classToHandle = new GetCountryById
        {
            Id = CountryMockBuilder.GenerateMockCountry().Id
        };

        var handler = (GetCountryByIdHandler)CountryMockBuilder.CreateHandler<GetCountryByIdHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void GetCountryByClientIdInvalidRangeTest()
    {
        var classToHandle = new GetCountryById();
        
        var handler = (GetCountryByIdHandler)CountryMockBuilder.CreateHandler<GetCountryByIdHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
