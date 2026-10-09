using FastEndpoints;
using FluentValidation;

namespace DiscoveryService.Backend.Accommodations;

/// <summary>Enforces the required fields of the <c>Accommodation</c> and <c>Location</c> schemas.</summary>
public sealed class AccommodationValidator : Validator<Accommodation>
{
    public AccommodationValidator()
    {
        RuleFor(a => a.Name).NotEmpty();
        RuleFor(a => a.Amenities).NotNull();
        RuleFor(a => a.Location).NotNull();
        RuleFor(a => a.Location.City).NotEmpty().When(a => a.Location is not null);
        RuleFor(a => a.Location.Country).NotEmpty().When(a => a.Location is not null);
    }
}
