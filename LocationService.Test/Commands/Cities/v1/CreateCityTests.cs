using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Cities.Commands.v1;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using LocationService.Test.MockBuilder;
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
    public void CreateCityInvalidNameTest()
    {
        // Arrange
        var resultDto = CityMockBuilder.GenerateMockCityDtoList(1).First();
        resultDto.Name = null;
        
        var classToHandle = new CreateCity
        {
            LocationDetails = resultDto
        };
        
        var handler = (CreateCityHandler)CityMockBuilder.CreateHandler<CreateCityHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
