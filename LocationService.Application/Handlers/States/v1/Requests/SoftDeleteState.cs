using MediatR;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class SoftDeleteState : IRequest<string>
{
    public string Id { get; init; }
}
