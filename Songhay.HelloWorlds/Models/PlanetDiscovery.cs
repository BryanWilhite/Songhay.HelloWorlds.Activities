namespace Songhay.HelloWorlds.Models;

public record struct PlanetDiscovery
{
    public int Id { get; set;}

    public string DiscoveryMethod { get; set;}

    public int DiscoveryYear { get; set;}

    public int PublicationMonth { get; set;}

    public int PublicationYear { get; set;}
}
