using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Cities.Commands.v1;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using LocationService.Test.MockBuilder;
using LocationService.Test.Queries.Cities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.Cities.v1;

[TestClass]
public class UpdateCityTests
{
    [TestMethod]
    public async Task UpdateCityTest()
    {
        // Arrange
        var classToHandle = new UpdateCity
        {
            LocationDetails = CityMockBuilder.GenerateMockCityDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateCityHandler)CityMockBuilder.CreateHandler<UpdateCityHandler>();
        
        //Act
        var result = (CityData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void UpdateCityInvalidCityIdTest()
    {
        // Arrange
        var classToHandle = new UpdateCity
        {
            LocationDetails = new CityData()
        };

        var handler = (UpdateCityHandler)CityMockBuilder.CreateHandler<UpdateCityHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
