using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Definition.States.Requests.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Commands.v1;

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
        var entity = await _repository.GetAsSingleAsync<State, string>(c => c.Id == stateId || c.Code == stateId);
        if (entity == null) return null;
            
        await _repository.DeleteAsync(entity);
        return entity;
    }
}
