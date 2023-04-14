using LocationService.Message.Contracts.Countries.v1;
using MediatR;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class CreateCountry : IRequest<CountryData>
{
    public CountryData Details { get; init; }
}