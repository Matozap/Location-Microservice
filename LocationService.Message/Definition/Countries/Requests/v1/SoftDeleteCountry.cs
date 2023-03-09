using MediatR;

namespace LocationService.Message.Definition.Countries.Requests.v1;

public class SoftDeleteCountry : BaseMessage, IRequest<object>
{
    public string Id { get; init; }
}
