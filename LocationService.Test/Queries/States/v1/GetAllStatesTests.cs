using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Handlers.States.Queries;
using LocationService.Message.States;
using LocationService.Message.States.Requests;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.States.v1;

public class GetAllStatesTests
{
    [Fact]
    public async Task GetAllStatesTest()
    {
        var classToHandle = new GetAllStates
        {
            CountryId = "CO"
        };
        
        var handler = (GetAllStatesHandler)StateMockBuilder.CreateHandler<GetAllStatesHandler>();
        var result = (List<StateData>)await handler.Handle(classToHandle, new CancellationToken());

        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
