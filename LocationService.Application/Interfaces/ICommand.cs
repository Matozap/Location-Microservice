using MediatR;

namespace LocationService.Application.Interfaces;

public interface ICommand<out TCommandResult> : IRequest<TCommandResult>
{
}