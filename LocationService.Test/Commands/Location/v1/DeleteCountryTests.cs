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
public class DeleteCountryTests
{
    [TestMethod]
    public async Task DeleteCountryTest()
    {
        // Arrange
        var classToHandle = new DeleteCountry
        {
            CountryId = CountryMockBuilder.GenerateMockCountry().Id
        };

        
        var handler = (DeleteCountryHandler)CountryMockBuilder.CreateHandler<DeleteCountryHandler>();

        //Act
        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void DeleteCountryInvalidCountryIdTest()
    {
        // Arrange
        var classToHandle = new DeleteCountry
        {
            CountryId = null
        };

        var handler = (DeleteCountryHandler)CountryMockBuilder.CreateHandler<DeleteCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
