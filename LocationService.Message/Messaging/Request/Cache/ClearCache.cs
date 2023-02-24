using MediatR;

namespace LocationService.Message.Messaging.Request.Cache;

public class ClearCache : BaseMessage, IRequest<bool>
{
    public string CountryId { get; init; }
    public int StateId { get; init; }
    public string StateCode { get; set; }
    public int CityId { get; init; }
    public bool ClearAll { get; set; }
}