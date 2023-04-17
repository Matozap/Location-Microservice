using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class SoftDeleteState : ICommand<string>
{
    public string Id { get; init; }
}
