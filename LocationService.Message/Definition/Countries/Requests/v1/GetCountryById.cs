using MediatR;

namespace LocationService.Message.Definition.Countries.Requests.v1;

public class GetCountryById : BaseMessage, IRequest<object>
{
    public string Id { get; init; }
}
