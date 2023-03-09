using LocationService.Message.DataTransfer.Countries.v1;
using MediatR;

namespace LocationService.Message.Definition.Countries.Responses.v1;

public class CountryUpdated : IRequest<object>
{
    public CountryData LocationDetails { get; init; }
}
