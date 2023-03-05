using MediatR;

namespace LocationService.Message.Messaging.Request.Cities.v1;

public class SoftDeleteCity : BaseMessage, IRequest<object>
{
    public int Id { get; init; }
}
