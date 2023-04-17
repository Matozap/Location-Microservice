using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Cities.v1;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class CreateCity : ICommand<CityData>
{
    public CityData Details { get; init; }
}