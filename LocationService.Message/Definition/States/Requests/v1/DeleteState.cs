using MediatR;

namespace LocationService.Message.Definition.States.Requests.v1;

public class DeleteState : BaseMessage, IRequest<string>
{
    public string Id { get; init; }
}
