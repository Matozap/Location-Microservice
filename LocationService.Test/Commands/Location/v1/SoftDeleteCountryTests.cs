using LocationService.Test.Mocking;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Commands.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;

namespace LocationService.Test.Commands.Location.v1;

[TestClass]
public class SoftDeleteCountryTests
{
    [TestMethod]
    public async Task SoftDeleteCountryTest()
    {
        // Arrange
        var classToHandle = new SoftDeleteCountry
        {
            CountryId = CountryMockBuilder.GenerateMockCountry().Id
        };
        
        var handler = (SoftDeleteCountryHandler)CountryMockBuilder.CreateHandler<SoftDeleteCountryHandler>();

        //Act
        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void SoftDeleteCountryInvalidCountryIdTest()
    {
        // Arrange
        var classToHandle = new SoftDeleteCountry
        {
            CountryId = null
        };
        
        var handler = (SoftDeleteCountryHandler)CountryMockBuilder.CreateHandler<SoftDeleteCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
