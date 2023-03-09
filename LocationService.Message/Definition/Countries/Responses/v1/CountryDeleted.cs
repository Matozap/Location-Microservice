using MediatR;

namespace LocationService.Message.Definition.Countries.Responses.v1;

public class CountryDeleted : IRequest<object>
{
    public string CountryId { get; init; }
}
