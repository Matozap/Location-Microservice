using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Commands.Countries.v1;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using LocationService.Test.Queries.Countries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.Countries.v1;

[TestClass]
public class UpdateCountryTests
{
    [TestMethod]
    public async Task UpdateCountryTest()
    {
        // Arrange
        var classToHandle = new UpdateCountry
        {
            LocationDetails = CountryMockBuilder.GenerateMockCountryDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateCountryHandler)CountryMockBuilder.CreateHandler<UpdateCountryHandler>();
        
        //Act
        var result = (CountryData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void UpdateCountryInvalidCountryIdTest()
    {
        // Arrange
        var classToHandle = new UpdateCountry
        {
            LocationDetails = new CountryData()
        };

        var handler = (UpdateCountryHandler)CountryMockBuilder.CreateHandler<UpdateCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
