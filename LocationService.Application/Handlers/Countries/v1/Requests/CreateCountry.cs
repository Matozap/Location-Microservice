using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Countries.v1;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class CreateCountry : ICommand<CountryData>
{
    public CountryData Details { get; init; }
}
