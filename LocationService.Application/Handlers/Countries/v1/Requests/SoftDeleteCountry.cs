using MediatR;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class SoftDeleteCountry : IRequest<string>
{
    public string Id { get; init; }
}
