using MediatR;

namespace LocationService.Message.Contracts.Common.Interfaces;

public interface ICommand<out TCommandResult> : IRequest<TCommandResult>
{
}