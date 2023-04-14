using LocationService.Message.Contracts.Countries.v1;
using MediatR;

namespace LocationService.Application.Handlers.Countries.v1.Responses;

public class CountryUpdated : IRequest<object>
{
    public CountryData Details { get; init; }
}
