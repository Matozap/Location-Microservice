using MediatR;

namespace LocationService.Message.Messaging.Request.Countries.v1;

public class SoftDeleteCountry : BaseMessage, IRequest<object>
{
    public string CountryId { get; init; }
}
