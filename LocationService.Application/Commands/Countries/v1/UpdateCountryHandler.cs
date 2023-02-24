using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging;
using LocationService.Message.Messaging.Event;
using LocationService.Message.Messaging.Event.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Commands.Countries.v1;

public class UpdateCountryHandler : IRequestHandler<UpdateCountry, object>
{
    private readonly ILogger<UpdateCountryHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public UpdateCountryHandler(ILogger<UpdateCountryHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(UpdateCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.LocationDetails?.Id);
        
        await UpdateCountry(request.LocationDetails);
        _logger.LogInformation("Country with id {CountryID} created successfully", request.LocationDetails.Id);
        _ = _eventBus.Publish(new CountryEvent { LocationDetails = request.LocationDetails, Action = EventAction.CountryUpdate});
            
        return request.LocationDetails;
    }

    private async Task UpdateCountry(CountryFlatData countryData)
    {
        var query = new GetCountryById
        {
            CountryId = countryData.Id,
            Source = MessageSource.Command
        };
        var readResult = await _mediator.Send(query);
        var existingLocationDto = (CountryData)readResult;
            
        if(existingLocationDto != null)
        {                
            await _repository.UpdateAsync(countryData.Adapt<CountryFlatData, Domain.Country>());
        }
    }
}
