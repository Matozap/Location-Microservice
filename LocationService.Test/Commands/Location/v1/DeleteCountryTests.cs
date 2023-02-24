using LocationService.Test.Mocking;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
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
        var location = CountryMockBuilder.GenerateMockCountry();
        var locationDto = CountryMockBuilder.GenerateMockCountryDto();
        var classToHandle = new DeleteCountry
        {
            CountryId = location.Id
        };

        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCountryById>()).Returns(locationDto);

        var handler = new DeleteCountryHandler(NullLogger<DeleteCountryHandler>.Instance,
            CountryMockBuilder.GenerateMockRepository(location),
            mediator,
            CountryMockBuilder.GenerateMockEventBus());

        //Act
        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void DeleteCountryInvalidCountryIdTest()
    {
        // Arrange
        var location = CountryMockBuilder.GenerateMockCountry();
        var locationDto = CountryMockBuilder.GenerateMockCountryDto();
        location.Id = null;
        var classToHandle = new DeleteCountry
        {
            CountryId = location.Id
        };

        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCountryById>()).Returns(locationDto);

        var handler = new DeleteCountryHandler(NullLogger<DeleteCountryHandler>.Instance,
            CountryMockBuilder.GenerateMockRepository(location),
            mediator,
            CountryMockBuilder.GenerateMockEventBus());

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
