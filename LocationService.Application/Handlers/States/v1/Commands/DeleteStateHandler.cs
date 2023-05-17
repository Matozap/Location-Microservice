using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Handlers.States.v1.Requests;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.States.v1.Commands;

public class DeleteStateHandler : IRequestHandler<DeleteState, string>
{
    private readonly ILogger<DeleteStateHandler> _logger;
    private readonly IRepository _repository;

    public DeleteStateHandler(ILogger<DeleteStateHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(DeleteState request, CancellationToken cancellationToken)
    {
        var entity = await DeleteStateAsync(request.Id);
        _logger.LogInformation("State with id {StateId} deleted successfully", entity?.Id);
        
        return entity?.Id;
    }

    private async Task<State> DeleteStateAsync(string stateId)
    {
        var entity = await _repository.GetAsSingleAsync<State, string>(c => c.Id == stateId || c.Code == stateId, includeNavigationalProperties: true);
        if (entity == null) return null;
        
        if (entity.Cities?.Count > 0)
        {
            foreach (var city in entity.Cities.ToList())
            {
                await _repository.DeleteAsync(city);
            }
        }
            
        await _repository.DeleteAsync(entity);
        return entity;
    }
}
