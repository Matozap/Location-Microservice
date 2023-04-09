using LocationService.Message.DataTransfer.Countries.v1;
using MediatR;

namespace LocationService.Message.Definition.Countries.Requests.v1;

public class UpdateCountry : BaseMessage, IRequest<CountryData>
{
    public CountryData Details { get; init; }
}
