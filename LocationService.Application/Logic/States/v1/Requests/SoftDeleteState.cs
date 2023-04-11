using MediatR;

namespace LocationService.Application.Logic.States.v1.Requests;

public class SoftDeleteState : IRequest<string>
{
    public string Id { get; init; }
}
