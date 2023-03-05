using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Commands.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using LocationService.Test.Queries.Countries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.Countries.v1;

[TestClass]
public class SoftDeleteCountryTests
{
    [TestMethod]
    public async Task SoftDeleteCountryTest()
    {
        // Arrange
        var classToHandle = new SoftDeleteCountry
        {
            Id = CountryMockBuilder.GenerateMockCountry().Id
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
            Id = null
        };
        
        var handler = (SoftDeleteCountryHandler)CountryMockBuilder.CreateHandler<SoftDeleteCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
