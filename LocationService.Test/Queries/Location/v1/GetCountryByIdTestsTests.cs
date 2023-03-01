using LocationService.Test.Mocking;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Queries.Countries.v1;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;

namespace LocationService.Test.Queries.Location.v1;

[TestClass]
public class GetCountryByIdTestsTests
{
    [TestMethod]
    public async Task GetCountryByIdTestsTest()
    {
        // Arrange
        var classToHandle = new GetCountryById
        {
            CountryId = CountryMockBuilder.GenerateMockCountryFlatDto().Id
        };

        var handler = (GetCountryByIdHandler)CountryMockBuilder.CreateHandler<GetCountryByIdHandler>();
        
        //Act
        var result = (CountryData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void GetCountryByClientIdInvalidRangeTest()
    {
        // Arrange
        var classToHandle = new GetCountryById();
        
        var handler = (GetCountryByIdHandler)CountryMockBuilder.CreateHandler<GetCountryByIdHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
