using System.Threading.Tasks;

namespace LocationService.Application.Events.Publishers;

public interface IOutboxPublisher
{
    Task PublishOutboxAsync();
}