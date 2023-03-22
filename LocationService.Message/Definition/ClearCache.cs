using MediatR;

namespace LocationService.Message.Definition;

public class ClearCache : BaseMessage, IRequest<bool>
{
    public string CountryId { get; init; }
    public string StateId { get; init; }
    public string StateCode { get; init; }
    public string CityId { get; init; }
    public bool ClearAll { get; set; }
}