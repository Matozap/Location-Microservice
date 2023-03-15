using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class GetAllCities : BaseMessage, IRequest<object>
{
    public string StateId { get; set; }
}
