using LocationService.Message.DTO.Countries.v1;
using MediatR;

namespace LocationService.Message.Messaging.Response.Countries.v1;

public class CountryUpdated : IRequest<object>
{
    public CountryData LocationDetails { get; init; }
}
