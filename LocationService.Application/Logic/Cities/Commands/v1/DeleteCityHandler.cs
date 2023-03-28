using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Definition.Cities.Requests.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Commands.v1;

public class DeleteCityHandler : IRequestHandler<DeleteCity, object>
{
    private readonly ILogger<DeleteCityHandler> _logger;
    private readonly IRepository _repository;

    public DeleteCityHandler(ILogger<DeleteCityHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<object> Handle(DeleteCity request, CancellationToken cancellationToken)
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
