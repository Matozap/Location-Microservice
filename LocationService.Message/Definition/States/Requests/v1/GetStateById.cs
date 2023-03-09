using MediatR;

namespace LocationService.Message.Definition.States.Requests.v1;

public class GetStateById : BaseMessage, IRequest<object>
{
    public int Id { get; init; }
    public string Code { get; init; }
    
}
