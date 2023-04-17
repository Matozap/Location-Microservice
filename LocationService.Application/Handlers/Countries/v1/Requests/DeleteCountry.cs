using LocationService.Application.Interfaces;

namespace LocationService.Application.Handlers.Countries.v1.Requests;

public class DeleteCountry : ICommand<string>
{
    public string Id { get; init; }
}
