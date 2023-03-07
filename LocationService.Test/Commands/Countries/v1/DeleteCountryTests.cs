using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Commands.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using LocationService.Test.MockBuilder;
using LocationService.Test.Queries.Countries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.Countries.v1;

[TestClass]
public class DeleteCountryTests
{
    [TestMethod]
    public async Task DeleteCountryTest()
    {
        // Arrange
        var classToHandle = new DeleteCountry
        {
            Id = CountryMockBuilder.GenerateMockCountry().Id
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
            Id = null
        };

        var handler = (DeleteCountryHandler)CountryMockBuilder.CreateHandler<DeleteCountryHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
