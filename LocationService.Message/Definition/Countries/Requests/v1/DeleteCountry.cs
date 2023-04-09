using MediatR;

namespace LocationService.Message.Definition.Countries.Requests.v1;

public class DeleteCountry : BaseMessage, IRequest<string>
{
    public string Id { get; init; }
}
