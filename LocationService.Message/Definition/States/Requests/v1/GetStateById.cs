using MediatR;

namespace LocationService.Message.Definition.States.Requests.v1;

public class GetStateById : BaseMessage, IRequest<object>
{
    public string Id { get; init; }
    public string Code { get; init; }
    
}
