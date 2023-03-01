using LocationService.Test.Mocking;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Commands.Countries.v1;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;

namespace LocationService.Test.Commands.Location.v1;

[TestClass]
public class UpdateCountryTests
{
    [TestMethod]
    public async Task UpdateCountryTest()
    {
        // Arrange
        var classToHandle = new UpdateCountry
        {
            LocationDetails = CountryMockBuilder.GenerateMockCountryFlatDto()
        };

        var handler = (UpdateCountryHandler)CountryMockBuilder.CreateHandler<UpdateCountryHandler>();
        
        //Act
        var result = (CountryFlatData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void UpdateCountryInvalidCountryIdTest()
    {
        // Arrange
        var classToHandle = new UpdateCountry
        {
            LocationDetails = new CountryFlatData()
        };

        var handler = (UpdateCountryHandler)CountryMockBuilder.CreateHandler<UpdateCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
