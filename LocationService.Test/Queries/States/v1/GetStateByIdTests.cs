using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Queries.States.v1;
using LocationService.Message.DTO.States.v1;
using LocationService.Message.Messaging.Request.States.v1;
using LocationService.Test.MockBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Queries.States.v1;

[TestClass]
public class GetStateByIdTests
{
    [TestMethod]
    public async Task GetStateByIdTestsTest()
    {
        // Arrange
        var classToHandle = new GetStateById
        {
            Id = StateMockBuilder.GenerateMockState().Id
        };

        var handler = (GetStateByIdHandler)StateMockBuilder.CreateHandler<GetStateByIdHandler>();
        
        //Act
        var result = (StateData)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull();
    }

    [TestMethod]
    public void GetStateByClientIdInvalidRangeTest()
    {
        // Arrange
        var classToHandle = new GetStateById();
        
        var handler = (GetStateByIdHandler)StateMockBuilder.CreateHandler<GetStateByIdHandler>();

        //Act
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
