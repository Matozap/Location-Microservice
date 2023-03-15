using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class DeleteCity : BaseMessage, IRequest<object>
{
    public string Id { get; init; }
}
