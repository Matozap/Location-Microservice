using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class GetStateById : IQuery<object>
{
    public string Id { get; init; }
    public string Code { get; init; }
}
