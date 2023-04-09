using LocationService.Message.DataTransfer.Countries.v1;
using MediatR;

namespace LocationService.Message.Definition.Countries.Requests.v1;

public class GetCountryById : BaseMessage, IRequest<CountryData>
{
    public string Id { get; init; }
}
