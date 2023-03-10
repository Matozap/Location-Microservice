using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Countries.Events.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.Commands.v1;

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

    private async Task UpdateCountry(CountryData countryData)
    {
        var entity = await _repository.GetCountryAsync(c => c.Id == countryData.Id);
        if(entity != null)
        {                
            await _repository.UpdateAsync(countryData.Adapt<CountryData, Domain.Country>());
        }
    }
}
