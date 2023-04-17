using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class GetAllStates : IQuery<object>
{
    public string CountryId { get; init; }
}
