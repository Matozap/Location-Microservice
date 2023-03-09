using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class SoftDeleteCity : BaseMessage, IRequest<object>
{
    public int Id { get; init; }
}
