using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class DeleteCity : ICommand<string>
{
    public string Id { get; init; }
}
