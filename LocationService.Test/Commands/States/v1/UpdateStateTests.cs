using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.States.v1.Commands;
using LocationService.Application.Logic.States.v1.Requests;
using LocationService.Message.Definition.Protos.States.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.States.v1;

public class UpdateStateTests
{
    [Fact]
    public async Task UpdateStateTest()
    {
        var classToHandle = new UpdateState
        {
            Details = StateMockBuilder.GenerateMockStateDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateStateHandler)StateMockBuilder.CreateHandler<UpdateStateHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void UpdateStateInvalidStateIdTest()
    {
        var classToHandle = new UpdateState
        {
            Details = new StateData()
        };

        var handler = (UpdateStateHandler)StateMockBuilder.CreateHandler<UpdateStateHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
