using MediatR;

namespace LocationService.Message.Definition.States.Requests.v1;

public class GetAllStates : BaseMessage, IRequest<object>
{
    public string CountryId { get; init; }
}
