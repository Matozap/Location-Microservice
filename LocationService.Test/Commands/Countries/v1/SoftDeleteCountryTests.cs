using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Countries.Commands;
using LocationService.Application.Countries.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Countries.v1;

public class SoftDeleteCountryTests
{
    [Fact]
    public async Task SoftDeleteCountryTest()
    {
        var classToHandle = new SoftDeleteCountry
        {
            Id = CountryMockBuilder.GenerateMockCountry().Id
        };
        
        var handler = (SoftDeleteCountryHandler)CountryMockBuilder.CreateHandler<SoftDeleteCountryHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void SoftDeleteCountryInvalidCountryIdTest()
    {
        var classToHandle = new SoftDeleteCountry
        {
            Id = null
        };
        
        var handler = (SoftDeleteCountryHandler)CountryMockBuilder.CreateHandler<SoftDeleteCountryHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
