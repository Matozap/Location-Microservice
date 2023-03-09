using MediatR;

namespace LocationService.Message.Definition.States.Responses.v1;

public class StateDeleted : IRequest<object>
{
    public string StateId { get; init; }
}
