using System;
using System.ComponentModel.DataAnnotations;
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
public class CreateStateTests
{
    [TestMethod]
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

    [TestMethod]
    public void CreateStateInvalidNameTest()
    {
        // Arrange
        var locationDto = StateMockBuilder.GenerateMockStateDtoList(1).First();
        locationDto.Name = null;
        
        var classToHandle = new CreateState
        {
            LocationDetails = locationDto
        };
        
        var handler = (CreateStateHandler)StateMockBuilder.CreateHandler<CreateStateHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ValidationException>().WithMessage("*Name*");
    }
}
