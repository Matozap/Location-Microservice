using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.Countries.v1.Requests;
using LocationService.Domain;
using LocationService.Message.Contracts.Countries.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.v1.Commands;

public class UpdateCountryHandler : IRequestHandler<UpdateCountry, CountryData>
{
    private readonly ILogger<UpdateCountryHandler> _logger;
    private readonly IRepository _repository;

    public UpdateCountryHandler(ILogger<UpdateCountryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<CountryData> Handle(UpdateCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Id);
        
        var result = await UpdateCountry(request.Details);
        _logger.LogInformation("Country with id {CountryID} updated successfully", request.Details.Id);
            
        return result;
    }

    private async Task<CountryData> UpdateCountry(CountryData countryData)
    {
        var entity = await _repository.GetAsSingleAsync<Country, string>(e => e.Id == countryData.Id || e.Code == countryData.Code);
        if (entity == null) return null;
        
        var changes = countryData.Adapt(entity);
        await _repository.UpdateAsync(changes);
        return changes.Adapt<Country, CountryData>();
    }
}
