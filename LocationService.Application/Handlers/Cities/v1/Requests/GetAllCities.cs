using System.Collections.Generic;
using LocationService.Message.Contracts.Cities.v1;
using MediatR;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class GetAllCities : IRequest<List<CityData>>
{
    public string StateId { get; init; }
}
