using LocationService.Message.Contracts.Cities.v1;
using MediatR;

namespace LocationService.Application.Logic.Cities.v1.Requests;

public class CreateCity : IRequest<CityData>
{
    public CityData Details { get; init; }
}