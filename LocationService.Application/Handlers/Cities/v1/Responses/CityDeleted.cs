using MediatR;

namespace LocationService.Application.Handlers.Cities.v1.Responses;

public class CityDeleted : IRequest<object>
{
    public string CityId { get; init; }
}
