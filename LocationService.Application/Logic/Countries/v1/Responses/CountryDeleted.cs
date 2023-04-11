using MediatR;

namespace LocationService.Application.Logic.Countries.v1.Responses;

public class CountryDeleted : IRequest<object>
{
    public string CountryId { get; init; }
}
