using LocationService.Test.Mocking;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Queries.Countries.v1;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;

namespace LocationService.Test.Queries.Location.v1;

[TestClass]
public class GetAllCountriesTests
{
    [TestMethod]
    public async Task GetAllCountriesTest()
    {
        // Arrange
        const int numberOfRows = 10;
        var classToHandle = new GetAllCountries();

        var handler = new GetAllCountriesHandler(CountryMockBuilder.GenerateMockObjectCache(),
            NullLogger<GetAllCountriesHandler>.Instance,
            CountryMockBuilder.GenerateMockRepository(rowCount: numberOfRows));

        //Act
        var result = (List<CountryData>)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.HaveCount(numberOfRows);
    }
}
