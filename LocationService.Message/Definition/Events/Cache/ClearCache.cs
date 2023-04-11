using MediatR;

namespace LocationService.Message.Definition.Events.Cache;

public class ClearCache : IRequest<bool>
{
    public string CountryId { get; init; }
    public string StateId { get; init; }
    public string StateCode { get; init; }
    public string CityId { get; init; }
    public bool ClearAll { get; set; }
}