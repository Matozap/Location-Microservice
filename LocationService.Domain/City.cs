namespace LocationService.Domain;

public class City : EntityBase
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Disabled { get; set; }
    public string StateId { get; set; }
    public virtual State State { get; set; }
}