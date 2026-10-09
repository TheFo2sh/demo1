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
    public string? Description { get; set; }
    public double? Rating { get; set; }
    [Required]
    public Location Location { get; set; } = default!;
    [Required]
    public List<string> Amenities { get; set; } = default!;

    /// <summary>A deep copy, so a stored accommodation cannot be changed through the submitted instance.</summary>
    public Accommodation Copy() => new()
    {
        Name = Name,
        Description = Description,
        Rating = Rating,
        Location = Location.Copy(),
        Amenities = [.. Amenities]
    };
}

/// <summary>Where an accommodation is. Mirrors the <c>Location</c> schema.</summary>
public sealed class Location
{
    public string? Address { get; set; }
    [Required]
    public string City { get; set; } = default!;
    public string? PostalCode { get; set; }
    [Required]
    public string Country { get; set; } = default!;

    public Location Copy() => new()
    {
        Address = Address,
        City = City,
        PostalCode = PostalCode,
        Country = Country
    };
}
