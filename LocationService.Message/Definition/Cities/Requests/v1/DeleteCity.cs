using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class DeleteCity : BaseMessage, IRequest<string>
{
    public string Id { get; init; }
}
