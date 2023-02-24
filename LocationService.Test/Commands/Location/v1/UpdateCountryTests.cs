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
        var location = CountryMockBuilder.GenerateMockCountry();
        var locationDto = CountryMockBuilder.GenerateMockCountryDto();
        var classToHandle = new UpdateCountry
        {
            LocationDetails = locationDto
        };

        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCountryById>()).Returns(locationDto);

        var handler = new UpdateCountryHandler(NullLogger<UpdateCountryHandler>.Instance,
            CountryMockBuilder.GenerateMockRepository(location),
            mediator,
            CountryMockBuilder.GenerateMockEventBus());

        //Act
        var result = (CountryData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.BeOfType<CountryData>();
    }

    [TestMethod]
    public void UpdateCountryInvalidCountryIdTest()
    {
        // Arrange
        var location = CountryMockBuilder.GenerateMockCountry();
        var locationDto = CountryMockBuilder.GenerateMockCountryDto();
        locationDto.Id = null;
        var classToHandle = new UpdateCountry
        {
            LocationDetails = locationDto
        };

        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCountryById>()).Returns(locationDto);

        var handler = new UpdateCountryHandler(NullLogger<UpdateCountryHandler>.Instance,
            CountryMockBuilder.GenerateMockRepository(location),
            mediator,
            CountryMockBuilder.GenerateMockEventBus());

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Country Id*");
    }
}
