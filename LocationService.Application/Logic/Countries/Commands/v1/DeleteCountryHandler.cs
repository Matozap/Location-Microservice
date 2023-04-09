using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Definition.Countries.Requests.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.Commands.v1;

public class DeleteCountryHandler : IRequestHandler<DeleteCountry, string>
{
    private readonly ILogger<DeleteCountryHandler> _logger;
    private readonly IRepository _repository;

    public DeleteCountryHandler(ILogger<DeleteCountryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(DeleteCountry request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Id);

        var entity = await DeleteCountryAsync(request.Id);
        _logger.LogInformation("Country with id {CountryID} deleted successfully", entity?.Id);

        return entity?.Id;
    }

    private async Task<Country> DeleteCountryAsync(string id)
    {
        var entity = await _repository.GetAsSingleAsync<Country, string>(c => c.Id == id || c.Code == id);
            
        if(entity != null)
        {                
            await _repository.DeleteAsync(entity);
        }

        return entity;
    }
}
