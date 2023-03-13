using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.States.Queries.v1;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.States.v1;

public class GetStateByIdTests
{
    [Fact]
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

    [Fact]
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
