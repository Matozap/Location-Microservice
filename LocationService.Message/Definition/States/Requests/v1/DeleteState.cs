using MediatR;

namespace LocationService.Message.Definition.States.Requests.v1;

public class DeleteState : BaseMessage, IRequest<object>
{
    public string Id { get; init; }
}
