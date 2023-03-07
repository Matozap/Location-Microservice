using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Commands.States.v1;
using LocationService.Message.Messaging.Request.States.v1;
using LocationService.Test.MockBuilder;
using LocationService.Test.Queries.States;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.States.v1;

[TestClass]
public class SoftDeleteStateTests
{
    [TestMethod]
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
