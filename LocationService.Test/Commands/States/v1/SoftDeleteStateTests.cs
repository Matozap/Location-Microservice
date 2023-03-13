using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.States.Commands.v1;
using LocationService.Message.Definition.States.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Commands.States.v1;

public class SoftDeleteStateTests
{
    [Fact]
    public async Task SoftDeleteStateTest()
    {
        // Arrange
        var classToHandle = new SoftDeleteState
        {
            Id = StateMockBuilder.GenerateMockState().Id
        };
        
        var handler = (SoftDeleteStateHandler)StateMockBuilder.CreateHandler<SoftDeleteStateHandler>();

        //Act
        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }
}
