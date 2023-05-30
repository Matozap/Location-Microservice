using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.States;
using LocationService.Application.States.Queries;
using LocationService.Application.States.Requests;
using LocationService.Application.States.Responses;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.States.v1;

public class GetStateByIdTests
{
    [Fact]
    public async Task GetStateByIdTestsTest()
    {
        var classToHandle = new GetStateById
        {
            Id = StateMockBuilder.GenerateMockState().Id
        };

        var handler = (GetStateByIdHandler)StateMockBuilder.CreateHandler<GetStateByIdHandler>();
        var result = (StateData)await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull();
    }

    [Fact]
    public void GetStateByClientIdInvalidRangeTest()
    {
        var classToHandle = new GetStateById();
        
        var handler = (GetStateByIdHandler)StateMockBuilder.CreateHandler<GetStateByIdHandler>();
        Func<Task> action = async () => await handler.Handle(classToHandle, new CancellationToken());

        action.Should().ThrowAsync<ArgumentNullException>();
    }
}
