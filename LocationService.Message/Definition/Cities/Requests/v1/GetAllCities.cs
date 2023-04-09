using System.Collections.Generic;
using LocationService.Message.DataTransfer.Cities.v1;
using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class GetAllCities : BaseMessage, IRequest<List<CityData>>
{
    public string StateId { get; init; }
}
