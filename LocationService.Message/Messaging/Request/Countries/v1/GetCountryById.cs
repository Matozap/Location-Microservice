using MediatR;

namespace LocationService.Message.Messaging.Request.Countries.v1;

public class GetCountryById : BaseMessage, IRequest<object>
{
    public string Id { get; init; }
}
