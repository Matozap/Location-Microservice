using LocationService.Test.Mocking;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        var classToHandle = new CreateCountry
        {
            LocationDetails = CountryMockBuilder.GenerateMockCountryDtoList(1).First()
        };

        var handler = (CreateCountryHandler)CountryMockBuilder.CreateHandler<CreateCountryHandler>();

        //Act
        var result = (CountryData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.BeOfType<CountryData>();
    }

    [TestMethod]
    public void CreateCountryInvalidNameTest()
    {
        // Arrange
        var locationDto = CountryMockBuilder.GenerateMockCountryDtoList(1).First();
        locationDto.Name = null;
        
        var classToHandle = new CreateCountry
        {
            LocationDetails = locationDto
        };
        
        var handler = (CreateCountryHandler)CountryMockBuilder.CreateHandler<CreateCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
