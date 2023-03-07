using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Queries.Cities.v1;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging.Request.Cities.v1;
using LocationService.Test.MockBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Queries.Cities.v1;

[TestClass]
public class GetCityByIdTests
{
    [TestMethod]
    public async Task GetCityByIdTestsTest()
    {
        // Arrange
        var classToHandle = new GetCityById
        {
            Id = CityMockBuilder.GenerateMockCity().Id
        };

        var handler = (GetCityByIdHandler)CityMockBuilder.CreateHandler<GetCityByIdHandler>();
        
        //Act
        var result = (CityData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void GetCityByClientIdInvalidRangeTest()
    {
        // Arrange
        var classToHandle = new GetCityById();
        
        var handler = (GetCityByIdHandler)CityMockBuilder.CreateHandler<GetCityByIdHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
