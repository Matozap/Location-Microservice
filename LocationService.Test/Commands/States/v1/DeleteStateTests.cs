using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Handlers.States.v1.Commands;
using LocationService.Message.Contracts.States.v1.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.States.v1;

public class DeleteStateTests
{
    [Fact]
    public async Task DeleteStateTest()
    {
        var classToHandle = new DeleteState
        {
            Id = StateMockBuilder.GenerateMockState().Id
        };

        var handler = (DeleteStateHandler)StateMockBuilder.CreateHandler<DeleteStateHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }
}
