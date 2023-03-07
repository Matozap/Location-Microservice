using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Commands.States.v1;
using LocationService.Message.DTO.States.v1;
using LocationService.Message.Messaging.Request.States.v1;
using LocationService.Test.MockBuilder;
using LocationService.Test.Queries.States;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Commands.States.v1;

[TestClass]
public class UpdateStateTests
{
    [TestMethod]
    public async Task UpdateStateTest()
    {
        // Arrange
        var classToHandle = new UpdateState
        {
            LocationDetails = StateMockBuilder.GenerateMockStateDtoList(1).FirstOrDefault()
        };

        var handler = (UpdateStateHandler)StateMockBuilder.CreateHandler<UpdateStateHandler>();
        
        //Act
        var result = (StateData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void UpdateStateInvalidStateIdTest()
    {
        // Arrange
        var classToHandle = new UpdateState
        {
            LocationDetails = new StateData()
        };

        var handler = (UpdateStateHandler)StateMockBuilder.CreateHandler<UpdateStateHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>().WithMessage("*Id*");
    }
}
