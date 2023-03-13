using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.States.Commands.v1;
using LocationService.Message.Definition.States.Requests.v1;
using LocationService.Test.MockBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.States.v1;

[TestClass]
public class DeleteStateTests
{
    [TestMethod]
    public async Task DeleteStateTest()
    {
        // Arrange
        var classToHandle = new DeleteState
        {
            Id = StateMockBuilder.GenerateMockState().Id
        };

        
        var handler = (DeleteStateHandler)StateMockBuilder.CreateHandler<DeleteStateHandler>();

        //Act
        var result = await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }
}
