using FastEndpoints;

namespace DiscoveryService.Backend.Accommodations;

/// <summary>
/// <c>POST /accommodations</c> (operationId <c>ProvideAccommodation</c>): lets an
/// accommodation information provider submit an <see cref="Accommodation"/>.
/// </summary>
public sealed class ProvideAccommodationEndpoint : Endpoint<Accommodation>
{
    public override void Configure()
    {
        Post("/accommodations");
        AllowAnonymous();
        Description(b => b
            .WithName("ProvideAccommodation")
            .Accepts<Accommodation>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/problem+json"),
            clearDefaults: true);
        Summary(s =>
        {
            s.Summary = "Provide an accommodation";
            s.Description = "Submits an accommodation from an accommodation information provider to DiscoveryService.";
            s.Responses[StatusCodes.Status204NoContent] = "The accommodation was received.";
            s.Responses[StatusCodes.Status400BadRequest] = "The body is missing required Accommodation or Location fields.";
        });
    }

    public override async Task HandleAsync(Accommodation req, CancellationToken ct)
    {
        await Send.NoContentAsync(ct);
    }
}
