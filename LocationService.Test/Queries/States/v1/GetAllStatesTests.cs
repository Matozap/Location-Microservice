using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LocationService.Application.Queries.States.v1;
using LocationService.Message.DTO.States.v1;
using LocationService.Message.Messaging.Request.States.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationService.Test.Queries.States.v1;

[TestClass]
public class GetAllStatesTests
{
    [TestMethod]
    public async Task GetAllStatesTest()
    {
        // Arrange
        var classToHandle = new GetAllStates()
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
