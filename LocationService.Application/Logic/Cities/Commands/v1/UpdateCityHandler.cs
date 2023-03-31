using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Commands.v1;

public class UpdateCityHandler : IRequestHandler<UpdateCity, object>
{
    private readonly ILogger<UpdateCityHandler> _logger;
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public UpdateCityHandler(ILogger<UpdateCityHandler> logger, IRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(UpdateCity request, CancellationToken cancellationToken)
    {
        var result = await UpdateCity(request.Details);
        _logger.LogInformation("City with id {CityID} updated successfully", request.Details.Id);
            
        return result;
    }

    private async Task<CityData> UpdateCity(CityData cityData)
    {
        var entity = await _repository.GetAsSingleAsync<City, string>(city => city.Id == cityData.Id);
        if (entity == null) return null;
        
        cityData.StateId = entity.StateId;
        var changes = cityData.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        return changes.Adapt<City, CityData>();
    }
}
