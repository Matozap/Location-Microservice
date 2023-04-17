using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class DeleteState : ICommand<string>
{
    public string Id { get; init; }
}
