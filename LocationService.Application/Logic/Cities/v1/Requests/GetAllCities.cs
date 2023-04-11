using System.Collections.Generic;
using LocationService.Message.Definition.Protos.Cities.v1;
using MediatR;

namespace LocationService.Application.Logic.Cities.v1.Requests;

public class GetAllCities : IRequest<List<CityData>>
{
    public string StateId { get; init; }
}
