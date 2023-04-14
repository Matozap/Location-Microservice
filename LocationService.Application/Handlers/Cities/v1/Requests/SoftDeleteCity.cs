using MediatR;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class SoftDeleteCity : IRequest<string>
{
    public string Id { get; init; }
}
