using LocationService.Message.Contracts.Countries.v1;
using MediatR;

namespace LocationService.Application.Logic.Countries.v1.Requests;

public class GetCountryById : IRequest<CountryData>
{
    public string Id { get; init; }
}
