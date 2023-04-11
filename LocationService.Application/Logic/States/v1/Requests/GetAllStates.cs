using MediatR;

namespace LocationService.Application.Logic.States.v1.Requests;

public class GetAllStates : IRequest<object>
{
    public string CountryId { get; init; }
}
