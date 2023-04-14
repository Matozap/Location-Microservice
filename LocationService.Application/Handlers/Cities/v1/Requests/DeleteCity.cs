using MediatR;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class DeleteCity : IRequest<string>
{
    public string Id { get; init; }
}
