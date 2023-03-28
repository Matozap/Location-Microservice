using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.Cities.Commands.v1;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.Cities.v1;

public class CreateCityTests
{
    [Fact]
    public async Task CreateCityTest()
    {
        var classToHandle = new CreateCity
        {
            LocationDetails = CityMockBuilder.GenerateMockCityDtoList(1).First()
        };

        var handler = (CreateCityHandler)CityMockBuilder.CreateHandler<CreateCityHandler>();
        var result = (CityData)await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.BeOfType<CityData>();
    }

    [Fact]
    public void CreateCityInvalidNameTest()
    {
        var resultDto = CityMockBuilder.GenerateMockCityDtoList(1).First();
        resultDto.Name = null;
        
        var classToHandle = new CreateCity
        {
            LocationDetails = resultDto
        };
        
        var handler = (CreateCityHandler)CityMockBuilder.CreateHandler<CreateCityHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
