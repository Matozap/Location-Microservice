using MediatR;

namespace LocationService.Application.Interfaces;

public interface IQuery<out TIQueryResult> : IRequest<TIQueryResult>
{
    
}