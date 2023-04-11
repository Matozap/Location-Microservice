using LocationService.Message.Definition.Protos.Countries.v1;
using MediatR;

namespace LocationService.Application.Logic.Countries.v1.Requests;

public class UpdateCountry : IRequest<CountryData>
{
    public CountryData Details { get; init; }
}
