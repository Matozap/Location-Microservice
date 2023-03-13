using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Countries.Commands.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Countries.v1;

public class SoftDeleteCountryTests
{
    [Fact]
    public async Task SoftDeleteCountryTest()
    {
        // Arrange
        var classToHandle = new SoftDeleteCountry
        {
            Id = CountryMockBuilder.GenerateMockCountry().Id
        };
        
        var handler = (SoftDeleteCountryHandler)CountryMockBuilder.CreateHandler<SoftDeleteCountryHandler>();

        //Act
        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void SoftDeleteCountryInvalidCountryIdTest()
    {
        // Arrange
        var classToHandle = new SoftDeleteCountry
        {
            Id = null
        };
        
        var handler = (SoftDeleteCountryHandler)CountryMockBuilder.CreateHandler<SoftDeleteCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
