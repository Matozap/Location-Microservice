using MediatR;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class GetAllStates : IRequest<object>
{
    public string CountryId { get; init; }
}
