using LocationService.Message.DTO.Cities.v1;
using MediatR;

namespace LocationService.Message.Messaging.Response.Cities.v1;

public class CityUpdated : IRequest<object>
{
    public CityData LocationDetails { get; init; }
}
