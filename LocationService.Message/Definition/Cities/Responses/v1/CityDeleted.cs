using MediatR;

namespace LocationService.Message.Definition.Cities.Responses.v1;

public class CityDeleted : IRequest<object>
{
    public string CityId { get; init; }
}
