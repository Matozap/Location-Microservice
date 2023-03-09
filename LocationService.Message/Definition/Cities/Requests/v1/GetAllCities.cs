using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class GetAllCities : BaseMessage, IRequest<object>
{
    public int StateId { get; set; }
}
