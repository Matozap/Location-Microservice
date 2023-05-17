using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Handlers.Countries.v1.Requests;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Countries.v1.Commands;

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
        var entity = await _repository.GetAsSingleAsync<Country, string>(c => c.Id == id || c.Code == id, includeNavigationalProperties: true);

        if (entity == null) return null;

        if (entity.States?.Count > 0)
        {
            foreach (var state in entity.States.ToList())
            {
                if (state.Cities?.Count > 0)
                {
                    foreach (var city in state.Cities.ToList())
                    {
                        await _repository.DeleteAsync(city);
                    }
                }
                await _repository.DeleteAsync(state);
            }
        }

        await _repository.DeleteAsync(entity);

        return entity;
    }
}
