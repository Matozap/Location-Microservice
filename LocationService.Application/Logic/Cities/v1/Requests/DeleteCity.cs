using MediatR;

namespace LocationService.Application.Logic.Cities.v1.Requests;

public class DeleteCity : IRequest<string>
{
    public string Id { get; init; }
}
