using LocationService.Test.Mocking;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
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
        var location = CountryMockBuilder.GenerateMockCountry();
        var locationDto = CountryMockBuilder.GenerateMockCountryDto();
        var classToHandle = new GetCountryById
        {
            CountryId = locationDto.Id
        };

        var handler = new GetCountryByIdHandler(CountryMockBuilder.GenerateMockRepository(location),
            CountryMockBuilder.GenerateMockObjectCache(),
            NullLogger<GetCountryByIdHandler>.Instance);

        //Act
        var result = (CountryData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void GetCountryByClientIdInvalidRangeTest()
    {
        // Arrange
        var location = CountryMockBuilder.GenerateMockCountry();
        var classToHandle = new GetCountryById();

        var handler = new GetCountryByIdHandler(CountryMockBuilder.GenerateMockRepository(location),
            CountryMockBuilder.GenerateMockObjectCache(),
            NullLogger<GetCountryByIdHandler>.Instance);

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
