using MediatR;

namespace LocationService.Application.Logic.States.v1.Requests;

public class DeleteState : IRequest<string>
{
    public string Id { get; init; }
}
