using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.States.Commands;
using LocationService.Application.States.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.States.v1;

public class SoftDeleteStateTests
{
    [Fact]
    public async Task SoftDeleteStateTest()
    {
        var classToHandle = new SoftDeleteState
        {
            Id = StateMockBuilder.GenerateMockState().Id
        };
        
        var handler = (SoftDeleteStateHandler)StateMockBuilder.CreateHandler<SoftDeleteStateHandler>();
        var result = await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }
}
