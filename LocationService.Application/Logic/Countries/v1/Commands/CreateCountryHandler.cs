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

public class CreateCountryHandler : IRequestHandler<CreateCountry, CountryData>
{
    private readonly ILogger<CreateCountryHandler> _logger;
    private readonly IRepository _repository;

    public CreateCountryHandler(ILogger<CreateCountryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<CountryData> Handle(CreateCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Name);
        
        var resultEntity = await CreateCountry(request.Details);
        if (resultEntity == null) return null;
            
        _logger.LogInformation("Country with id {CountryID} created successfully", resultEntity.Id);
        return resultEntity.Adapt<Country, CountryData>();
    }

    private async Task<Country> CreateCountry(CountryData country)
    {
        if (await _repository.GetAsSingleAsync<Country,string>(e => e.Code == country.Code || e.Name == country.Name) != null)
        {
            return null;
        }
        
        var entity = country.Adapt<CountryData, Country>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
