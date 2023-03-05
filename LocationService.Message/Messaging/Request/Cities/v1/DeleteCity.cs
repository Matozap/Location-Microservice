using MediatR;

namespace LocationService.Message.Messaging.Request.Cities.v1;

public class DeleteCity : BaseMessage, IRequest<object>
{
    public int Id { get; init; }
}
