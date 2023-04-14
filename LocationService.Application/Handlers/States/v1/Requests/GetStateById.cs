using MediatR;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class GetStateById : IRequest<object>
{
    public string Id { get; init; }
    public string Code { get; init; }
    
}
