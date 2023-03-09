using MediatR;

namespace LocationService.Message.Definition.States.Requests.v1;

public class SoftDeleteState : BaseMessage, IRequest<object>
{
    public int Id { get; init; }
}
