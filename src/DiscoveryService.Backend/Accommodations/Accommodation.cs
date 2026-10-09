using System.ComponentModel.DataAnnotations;

namespace DiscoveryService.Backend.Accommodations;

/// <summary>
/// An accommodation as supplied by an accommodation information provider.
/// Mirrors the <c>Accommodation</c> schema of the DiscoveryService contract.
/// </summary>
public sealed class Accommodation
{
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public Location Location { get; set; } = default!;
    [Required]
    public List<string> Amenities { get; set; } = default!;
}

/// <summary>Where an accommodation is. Mirrors the <c>Location</c> schema.</summary>
public sealed class Location
{
    [Required]
    public string City { get; set; } = default!;
    [Required]
    public string Country { get; set; } = default!;
}
