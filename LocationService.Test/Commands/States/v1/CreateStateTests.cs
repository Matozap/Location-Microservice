using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.States.Commands.v1;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.States.v1;

public class CreateStateTests
{
    [Fact]
    public async Task CreateStateTest()
    {
        // Arrange
        var classToHandle = new CreateState
        {
            LocationDetails = StateMockBuilder.GenerateMockStateDtoList(1).First()
        };

        var handler = (CreateStateHandler)StateMockBuilder.CreateHandler<CreateStateHandler>();

        //Act
        var result = (StateData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.BeOfType<StateData>();
    }

    [Fact]
    public void CreateStateInvalidNameTest()
    {
        // Arrange
        var resultDto = StateMockBuilder.GenerateMockStateDtoList(1).First();
        resultDto.Name = null;
        
        var classToHandle = new CreateState
        {
            LocationDetails = resultDto
        };
        
        var handler = (CreateStateHandler)StateMockBuilder.CreateHandler<CreateStateHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
