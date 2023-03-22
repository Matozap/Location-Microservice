using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Logic.States.Queries.v1;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using LocationService.Test.MockBuilder;
using Xunit;

namespace LocationService.Test.Queries.States.v1;

public class GetAllStatesTests
{
    [Fact]
    public async Task GetAllStatesTest()
    {
        // Arrange
        var classToHandle = new GetAllStates
        {
            CountryId = "CO"
        };
        var handler = (GetAllStatesHandler)StateMockBuilder.CreateHandler<GetAllStatesHandler>();

        //Act
        var result = (List<StateData>)await handler.Handle(classToHandle, new CancellationToken());

        //Assert
        result.Should().NotBeNull().And.HaveCountGreaterThan(1);
    }
}
