namespace LocationService.Domain;

public class Outbox : EntityBase
{
    public string JsonObject { get; init; }
    public string ObjectType { get; init; }
    public Operation Operation { get; set; }
}

public enum Operation
{
    None,
    Create,
    Update,
    Delete
}