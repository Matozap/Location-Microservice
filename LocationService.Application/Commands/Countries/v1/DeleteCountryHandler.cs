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

public class DeleteCountryHandler : IRequestHandler<DeleteCountry, object>
{
    private readonly ILogger<DeleteCountryHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public DeleteCountryHandler(ILogger<DeleteCountryHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(DeleteCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.CountryId);

        await DeleteCountryAsync(request.CountryId);

        _ = _eventBus.Publish(new CountryEvent { LocationDetails = new CountryData {Id = request.CountryId}, Action = EventAction.CountryDelete});

        return request.CountryId;
    }

    private async Task DeleteCountryAsync(string countryId)
    {
        var query = new GetCountryById
        {
            CountryId = countryId,
            Source = MessageSource.Command
        };
        var readResult = await _mediator.Send(query);
        var existingLocationDto = (CountryData)readResult;
            
        if(existingLocationDto != null)
        {                
            await _repository.DeleteAsync(existingLocationDto.Adapt<CountryData, Domain.Country>());
            _logger.LogInformation("Country with id {CountryId} was completely deleted", countryId);
        }
    }
}
