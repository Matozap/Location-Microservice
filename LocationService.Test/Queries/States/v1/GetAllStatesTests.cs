using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.States.v1.Queries;
using LocationService.Application.Logic.States.v1.Requests;
using LocationService.Message.Definition.Protos.States.v1;
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
