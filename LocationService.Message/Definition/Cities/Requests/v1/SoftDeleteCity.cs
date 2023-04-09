using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class SoftDeleteCity : BaseMessage, IRequest<string>
{
    public string Id { get; init; }
}
