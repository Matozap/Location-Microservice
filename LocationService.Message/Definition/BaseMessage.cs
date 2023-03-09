namespace LocationService.Message.Definition;

public class BaseMessage
{
    public MessageSource Source { get; set; } = MessageSource.Default;
}
