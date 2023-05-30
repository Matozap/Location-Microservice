using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.States;
using LocationService.Application.States.Commands;
using LocationService.Application.States.Requests;
using LocationService.Application.States.Responses;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.States.v1;

public class CreateStateTests
{
    [Fact]
    public async Task CreateStateTest()
    {
        var classToHandle = new CreateState
        {
            Details = StateMockBuilder.GenerateMockStateDtoList(1).First()
        };

        var handler = (CreateStateHandler)StateMockBuilder.CreateHandler<CreateStateHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.BeOfType<StateData>();
    }

    [Fact]
    public void CreateStateInvalidNameTest()
    {
        var resultDto = StateMockBuilder.GenerateMockStateDtoList(1).First();
        resultDto.Name = "";
        
        var classToHandle = new CreateState
        {
            Details = resultDto
        };
        
        var handler = (CreateStateHandler)StateMockBuilder.CreateHandler<CreateStateHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());
    
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
