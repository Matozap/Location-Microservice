namespace LocationService.Domain;

public class City : EntityBase
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Disabled { get; set; }
    public int StateId { get; set; }
    public virtual State State { get; set; }
}