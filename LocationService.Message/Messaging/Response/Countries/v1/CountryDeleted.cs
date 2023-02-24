using MediatR;

namespace LocationService.Message.Messaging.Response.Countries.v1;

public class CountryDeleted : IRequest<object>
{
    public string CountryId { get; init; }
}
