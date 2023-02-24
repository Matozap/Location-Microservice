using LocationService.Message.DTO.Countries.v1;
using MediatR;

namespace LocationService.Message.Messaging.Request.Countries.v1;

public class CreateCountry : BaseMessage, IRequest<object>
{
    public CountryFlatData LocationDetails { get; init; }
}
