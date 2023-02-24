using MediatR;

namespace LocationService.Message.Messaging.Request.States.v1;

public class GetStateById : BaseMessage, IRequest<object>
{
    public int Id { get; init; }
    public string Code { get; init; }
    
}
