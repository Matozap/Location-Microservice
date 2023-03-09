using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Cities.Events.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Commands.v1;

public class CreateCityHandler : IRequestHandler<CreateCity, object>
{
    private readonly ILogger<CreateCityHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IEventBus _eventBus;

    public CreateCityHandler(ILogger<CreateCityHandler> logger, ILocationRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(CreateCity request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.LocationDetails?.Name);
        
        var resultEntity = await CreateCity(request.LocationDetails);
        if (resultEntity == null) return null;
        
        _logger.LogInformation("City with id {CityID} created successfully", resultEntity.Id.ToString());
        var resultDto = resultEntity.Adapt<Domain.City, CityData>();
            
        _ = _eventBus.Publish(new CityEvent { LocationDetails = request.LocationDetails, Action = EventAction.CityCreate});

        return resultDto;
    }

    private async Task<Domain.City> CreateCity(CityData city)
    {
        if (await _repository.GetCityAsync(e => e.Name == city.Name && e.StateId == city.StateId) != null)
        {
            return null;
        }
        
        var entity = city.Adapt<CityData, Domain.City>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
