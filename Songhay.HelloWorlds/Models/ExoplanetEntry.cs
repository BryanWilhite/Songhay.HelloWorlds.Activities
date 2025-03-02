namespace Songhay.HelloWorlds.Models;

public record ExoplanetEntry
{
    public required int Id { get; set; }

    public required string PlanetName { get; set; }

    public required string HostName { get; set; }

    public ArchiveDispositionEnum ArchiveDisposition { get; set; } = ArchiveDispositionEnum.CANDIDATE;

    public string? ArchiveDispositionReference { get; set; }

    public required decimal DistanceInParsecs { get; set; }

    public required Magnitude GaiaMagnitude{ get; set; }

    public required Magnitude JohnsonMagnitude{ get; set; }

    public required Magnitude TwoMassMagnitude{ get; set; }

    public int? NumberOfPlanets { get; set; }

    public int? NumberOfStars { get; set; }

    public required PlanetDiscovery PlanetDiscovery{ get; set; }

    public decimal? OrbitalPeriod { get; set; }

    public decimal? OrbitalSemiMajorAxis { get; set; }

    public decimal? PlanetMassWithRespectToEarth { get; set; }

    public decimal? PlanetMassWithRespectToJupiter { get; set; }

    public decimal? PlanetRadiusWithRespectToEarth { get; set; }

    public decimal? PlanetRadiusWithRespectToJupiter { get; set; }
}
