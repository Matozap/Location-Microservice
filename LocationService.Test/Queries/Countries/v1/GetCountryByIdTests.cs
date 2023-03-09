using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Countries.Queries.v1;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using LocationService.Test.MockBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Queries.Countries.v1;

[TestClass]
public class GetCountryByIdTests
{
    [TestMethod]
    public async Task GetCountryByIdTestsTest()
    {
        // Arrange
        var classToHandle = new GetCountryById
        {
            Id = CountryMockBuilder.GenerateMockCountry().Id
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
