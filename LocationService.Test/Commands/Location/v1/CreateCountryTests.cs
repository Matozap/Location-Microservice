using LocationService.Test.Mocking;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Commands.Countries.v1;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;

namespace LocationService.Test.Commands.Location.v1;

[TestClass]
public class CreateCountryTests
{
    [TestMethod]
    public async Task CreateCountryTest()
    {
        // Arrange
        var location = CountryMockBuilder.GenerateMockCountry();
        var locationDto = CountryMockBuilder.GenerateMockCountryDto();
        var classToHandle = new CreateCountry
        {
            LocationDetails = locationDto
        };

        var handler = new CreateCountryHandler(NullLogger<CreateCountryHandler>.Instance,
            CountryMockBuilder.GenerateMockRepository(location),
            CountryMockBuilder.GenerateMockEventBus());

        //Act
        var result = (CountryFlatData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.BeOfType<CountryFlatData>();
    }

    
    [TestMethod]
    public void CreateCountryInvalidNameTest()
    {
        // Arrange
        var location = CountryMockBuilder.GenerateMockCountry();
        var locationDto = CountryMockBuilder.GenerateMockCountryDto();
        locationDto.Name = null;
        var classToHandle = new CreateCountry
        {
            LocationDetails = locationDto
        };

        var handler = new CreateCountryHandler(NullLogger<CreateCountryHandler>.Instance,
            CountryMockBuilder.GenerateMockRepository(location),
            CountryMockBuilder.GenerateMockEventBus());

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
