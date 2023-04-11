using LocationService.Message.Contracts.Cities.v1;
using MediatR;

namespace LocationService.Application.Logic.Cities.v1.Responses;

public class CityCreated : IRequest<object>
{
    public CityData Details { get; init; }
}
