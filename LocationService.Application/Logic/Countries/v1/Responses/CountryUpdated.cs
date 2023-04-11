using LocationService.Message.Definition.Protos.Countries.v1;
using MediatR;

namespace LocationService.Application.Logic.Countries.v1.Responses;

public class CountryUpdated : IRequest<object>
{
    public CountryData Details { get; init; }
}
