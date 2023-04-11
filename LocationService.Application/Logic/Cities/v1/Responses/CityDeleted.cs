using MediatR;

namespace LocationService.Application.Logic.Cities.v1.Responses;

public class CityDeleted : IRequest<object>
{
    public string CityId { get; init; }
}
