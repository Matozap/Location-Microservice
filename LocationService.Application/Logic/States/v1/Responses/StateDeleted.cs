using MediatR;

namespace LocationService.Application.Logic.States.v1.Responses;

public class StateDeleted : IRequest<object>
{
    public string StateId { get; init; }
}
