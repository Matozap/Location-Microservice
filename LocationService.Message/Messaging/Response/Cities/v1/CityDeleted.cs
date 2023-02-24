using MediatR;

namespace LocationService.Message.Messaging.Response.Cities.v1;

public class CityDeleted : IRequest<object>
{
    public string CityId { get; init; }
}
