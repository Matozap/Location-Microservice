using LocationService.Message.Contracts.Cities.v1;
using MediatR;

namespace LocationService.Application.Logic.Cities.v1.Responses;

public class CityUpdated : IRequest<object>
{
    public CityData Details { get; init; }
}
