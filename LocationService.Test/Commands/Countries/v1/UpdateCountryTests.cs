using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Countries.v1.Commands;
using LocationService.Application.Logic.Countries.v1.Requests;
using LocationService.Message.Definition.Protos.Countries.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Countries.v1;

public class UpdateCountryTests
{
    [Fact]
    public async Task UpdateCountryTest()
    {
        var classToHandle = new UpdateCountry
        {
            Details = CountryMockBuilder.GenerateMockCountryDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateCountryHandler)CountryMockBuilder.CreateHandler<UpdateCountryHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void UpdateCountryInvalidCountryIdTest()
    {
        var classToHandle = new UpdateCountry
        {
            Details = new CountryData()
        };

        var handler = (UpdateCountryHandler)CountryMockBuilder.CreateHandler<UpdateCountryHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
