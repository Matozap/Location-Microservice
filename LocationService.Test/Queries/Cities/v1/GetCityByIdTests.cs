using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Cities.Queries.v1;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.Cities.v1;

public class GetCityByIdTests
{
    [Fact]
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

    [Fact]
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
