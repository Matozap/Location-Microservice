using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Countries.Commands.v1;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Countries.v1;

public class CreateCountryTests
{
    [Fact]
    public async Task CreateCountryTest()
    {
        var classToHandle = new CreateCountry
        {
            Details = CountryMockBuilder.GenerateMockCountryDtoList(1).First()
        };

        var handler = (CreateCountryHandler)CountryMockBuilder.CreateHandler<CreateCountryHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.BeOfType<CountryData>();
    }

    [Fact]
    public void CreateCountryInvalidNameTest()
    {
        var resultDto = CountryMockBuilder.GenerateMockCountryDtoList(1).First();
        resultDto.Name = "";
        
        var classToHandle = new CreateCountry
        {
            Details = resultDto
        };
        
        var handler = (CreateCountryHandler)CountryMockBuilder.CreateHandler<CreateCountryHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
