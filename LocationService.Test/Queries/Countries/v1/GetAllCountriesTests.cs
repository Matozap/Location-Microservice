using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Queries.Countries.v1;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Queries.Countries.v1;

[TestClass]
public class GetAllCountriesTests
{
    [TestMethod]
    public async Task GetAllCountriesTest()
    {
        // Arrange
        var classToHandle = new GetAllCountries();
        var handler = (GetAllCountriesHandler)CountryMockBuilder.CreateHandler<GetAllCountriesHandler>();

        //Act
        var result = (List<CountryData>)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
