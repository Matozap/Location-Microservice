using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Handlers.Cities.v1.Requests;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Cities.v1.Commands;

public class DeleteCityHandler : IRequestHandler<DeleteCity, string>
{
    private readonly ILogger<DeleteCityHandler> _logger;
    private readonly IRepository _repository;

    public DeleteCityHandler(ILogger<DeleteCityHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(DeleteCity request, CancellationToken cancellationToken)
    {
        var entity = await DeleteCityAsync(request.Id);
        _logger.LogInformation("City with id {CityId} deleted successfully", entity?.Id);
        
        return entity?.Id;
    }

    private async Task<City> DeleteCityAsync(string cityId)
    {
        var entity = await _repository.GetAsSingleAsync<City, string>(city => city.Id == cityId);
        if (entity == null) return null;
        
        await _repository.DeleteAsync(entity);
        return entity;
    }
}
