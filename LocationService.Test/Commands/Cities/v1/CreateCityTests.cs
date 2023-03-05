using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Commands.Cities.v1;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging.Request.v1;
using LocationService.Test.Queries.Cities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.Cities.v1;

[TestClass]
public class CreateCityTests
{
    [TestMethod]
    public async Task CreateCityTest()
    {
        // Arrange
        var classToHandle = new CreateCity
        {
            LocationDetails = CityMockBuilder.GenerateMockCityDtoList(1).First()
        };

        var handler = (CreateCityHandler)CityMockBuilder.CreateHandler<CreateCityHandler>();

        //Act
        var result = (CityData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.BeOfType<CityData>();
    }

    [TestMethod]
    public void CreateCitiyInvalidNameTest()
    {
        // Arrange
        var locationDto = CityMockBuilder.GenerateMockCityDtoList(1).First();
        locationDto.Name = null;
        
        var classToHandle = new CreateCity
        {
            LocationDetails = locationDto
        };
        
        var handler = (CreateCityHandler)CityMockBuilder.CreateHandler<CreateCityHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}