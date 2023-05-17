using MediatR;

namespace LocationService.Message.Contracts.Common.Interfaces;

public interface IQuery<out TIQueryResult> : IRequest<TIQueryResult>
{
    
}