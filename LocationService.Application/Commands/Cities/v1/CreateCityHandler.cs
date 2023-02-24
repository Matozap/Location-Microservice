using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging.Event;
using LocationService.Message.Messaging.Event.v1;
using LocationService.Message.Messaging.Request.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Commands.Cities.v1;

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
        
        var resultEntity = await CreateState(request.LocationDetails);
        _logger.LogInformation("City with id {CityID} created successfully", resultEntity.Id);
        var locationDto = resultEntity.Adapt<Domain.City, CityFlatData>();
            
        _ = _eventBus.Publish(new CityEvent { LocationDetails = request.LocationDetails, Action = EventAction.CityCreate});

        return locationDto;
    }

    private async Task<Domain.City> CreateState(CityFlatData city)
    {
        var entity = city.Adapt<CityFlatData, Domain.City>();
        entity.LastUpdateUserId = "system";
        entity.LastUpdateDate= DateTime.Now;
        return await _repository.AddAsync(entity);
    }
}
