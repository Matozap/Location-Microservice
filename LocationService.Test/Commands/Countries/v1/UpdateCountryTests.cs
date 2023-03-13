using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Countries.Commands.v1;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Countries.v1;

public class UpdateCountryTests
{
    [Fact]
    public async Task UpdateCountryTest()
    {
        // Arrange
        var classToHandle = new UpdateCountry
        {
            LocationDetails = CountryMockBuilder.GenerateMockCountryDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateCountryHandler)CountryMockBuilder.CreateHandler<UpdateCountryHandler>();
        
        //Act
        var result = (CountryData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void UpdateCountryInvalidCountryIdTest()
    {
        // Arrange
        var classToHandle = new UpdateCountry
        {
            LocationDetails = new CountryData()
        };

        var handler = (UpdateCountryHandler)CountryMockBuilder.CreateHandler<UpdateCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
