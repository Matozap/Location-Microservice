namespace LocationService.Message.Messaging;

public class BaseMessage
{
    public MessageSource Source { get; set; } = MessageSource.Default;
}
