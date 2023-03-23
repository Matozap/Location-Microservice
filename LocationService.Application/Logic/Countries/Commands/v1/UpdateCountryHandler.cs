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
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public UpdateCountryHandler(ILogger<UpdateCountryHandler> logger, IRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(UpdateCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.LocationDetails?.Id);
        
        var result = await UpdateCountry(request.LocationDetails);
        _logger.LogInformation("Country with id {CountryID} created successfully", request.LocationDetails.Id);
        _ = _eventBus.Publish(new CountryEvent { LocationDetails = request.LocationDetails, Action = EventAction.CountryUpdate});
            
        return result;
    }

    private async Task<CountryData> UpdateCountry(CountryData countryData)
    {
        var entity = await _repository.GetCountryAsync(e => e.Id == countryData.Id || e.Code == countryData.Code);
        if (entity == null) return null;
        
        var changes = countryData.Adapt(entity);
        await _repository.UpdateAsync(changes);
        return changes.Adapt<Domain.Country, CountryData>();
    }
}
