using System.Threading.Tasks;

namespace LocationService.Application.Interfaces;

public interface IEventBus
{
    Task Publish<T>(T message) where T : class;
}
