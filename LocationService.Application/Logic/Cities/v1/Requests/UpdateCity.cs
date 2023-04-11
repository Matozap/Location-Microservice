using LocationService.Message.Definition.Protos.Cities.v1;
using MediatR;

namespace LocationService.Application.Logic.Cities.v1.Requests;

public class UpdateCity : IRequest<CityData>
{
    public CityData Details { get; init; }
}
