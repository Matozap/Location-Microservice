using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Event;
using LocationService.Message.Messaging.Event.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Commands.Countries.v1;

public class CreateCountryHandler : IRequestHandler<CreateCountry, object>
{
    private readonly ILogger<CreateCountryHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IEventBus _eventBus;

    public CreateCountryHandler(ILogger<CreateCountryHandler> logger, ILocationRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(CreateCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.LocationDetails?.Id);
        ArgumentException.ThrowIfNullOrEmpty(request.LocationDetails?.Name);
        
        var resultEntity = await CreateCountry(request.LocationDetails);
        _logger.LogInformation("Country with id {CountryID} created successfully", resultEntity.Id);
        var locationDto = resultEntity.Adapt<Domain.Country, CountryFlatData>();
            
        _ = _eventBus.Publish(new CountryEvent { LocationDetails = request.LocationDetails, Action = EventAction.CountryCreate});

        return locationDto;
    }

    private async Task<Domain.Country> CreateCountry(CountryFlatData country)
    {
        var entity = country.Adapt<CountryFlatData, Domain.Country>();
        entity.LastUpdateUserId = "system";
        entity.LastUpdateDate= DateTime.Now;
        return await _repository.AddAsync(entity);
    }
}
