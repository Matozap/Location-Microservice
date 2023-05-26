using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Handlers.Countries.Commands;
using LocationService.Message.Countries.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Countries.v1;

public class DeleteCountryTests
{
    [Fact]
    public async Task DeleteCountryTest()
    {
        var classToHandle = new DeleteCountry
        {
            Id = CountryMockBuilder.GenerateMockCountry().Id
        };

        
        var handler = (DeleteCountryHandler)CountryMockBuilder.CreateHandler<DeleteCountryHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void DeleteCountryInvalidCountryIdTest()
    {
        var classToHandle = new DeleteCountry
        {
            Id = null
        };

        var handler = (DeleteCountryHandler)CountryMockBuilder.CreateHandler<DeleteCountryHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
