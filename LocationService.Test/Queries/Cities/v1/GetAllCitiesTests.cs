using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Queries.Cities.v1;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging.Request.Cities.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Queries.Cities.v1;

[TestClass]
public class GetAllCitiesTests
{
    [TestMethod]
    public async Task GetAllCitiesTest()
    {
        // Arrange
        var classToHandle = new GetAllCities()
        {
            StateId = 1
        };
        var handler = (GetAllCitiesHandler)CityMockBuilder.CreateHandler<GetAllCitiesHandler>();

        //Act
        var result = (List<CityData>)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
