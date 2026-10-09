using System.Net;
using System.Net.Http.Json;
using System.Text;
using DiscoveryService.Backend.Accommodations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DiscoveryService.Backend.Tests;

public sealed class ProvideAccommodationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private readonly HttpClient _client;

    public ProvideAccommodationTests() => _client = _factory.CreateClient();

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private IAccommodationStore Store => _factory.Services.GetRequiredService<IAccommodationStore>();

    private Task<HttpResponseMessage> PostJson(string json) =>
        _client.PostAsync("/accommodations", new StringContent(json, Encoding.UTF8, "application/json"));

    private const string ValidAccommodation = """
        {
          "name": "Nile View Hotel",
          "description": "Rooms overlooking the river",
          "rating": 4.5,
          "location": { "address": "1 Corniche St", "city": "Cairo", "postalCode": "11511", "country": "EG" },
          "amenities": ["wifi", "pool"]
        }
        """;

    [Fact]
    public async Task Valid_submission_returns_204_and_is_stored_with_all_supplied_fields()
    {
        var response = await PostJson(ValidAccommodation);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var stored = Assert.Single(Store.GetAll());
        Assert.Equal("Nile View Hotel", stored.Name);
        Assert.Equal("Rooms overlooking the river", stored.Description);
        Assert.Equal(4.5, stored.Rating);
        Assert.Equal("1 Corniche St", stored.Location.Address);
        Assert.Equal("Cairo", stored.Location.City);
        Assert.Equal("11511", stored.Location.PostalCode);
        Assert.Equal("EG", stored.Location.Country);
        Assert.Equal(["wifi", "pool"], stored.Amenities);
    }

    [Fact]
    public async Task Minimal_valid_submission_is_stored()
    {
        var response = await PostJson("""{ "name": "Hostel", "location": { "city": "Giza", "country": "EG" }, "amenities": [] }""");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var stored = Assert.Single(Store.GetAll());
        Assert.Equal("Hostel", stored.Name);
        Assert.Null(stored.Description);
        Assert.Null(stored.Rating);
        Assert.Empty(stored.Amenities);
    }

    [Theory]
    [InlineData("""{ "location": { "city": "Cairo", "country": "EG" }, "amenities": [] }""", "missing name")]
    [InlineData("""{ "name": "", "location": { "city": "Cairo", "country": "EG" }, "amenities": [] }""", "empty name")]
    [InlineData("""{ "name": "A", "amenities": [] }""", "missing location")]
    [InlineData("""{ "name": "A", "location": { "city": "Cairo", "country": "EG" } }""", "missing amenities")]
    [InlineData("""{ "name": "A", "location": { "country": "EG" }, "amenities": [] }""", "missing location.city")]
    [InlineData("""{ "name": "A", "location": { "city": "Cairo" }, "amenities": [] }""", "missing location.country")]
    [InlineData("""{}""", "empty object")]
    public async Task Submission_missing_a_required_field_returns_400_and_is_not_stored(string body, string because)
    {
        var response = await PostJson(body);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest, because);
        Assert.Empty(Store.GetAll());
    }

    [Fact]
    public async Task Concurrent_submissions_are_all_stored()
    {
        const int count = 200;

        var responses = await Task.WhenAll(Enumerable.Range(0, count).Select(i =>
            _client.PostAsJsonAsync("/accommodations", new
            {
                name = $"Hotel {i}",
                location = new { city = "Cairo", country = "EG" },
                amenities = new[] { $"amenity-{i}" }
            })));

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.NoContent, r.StatusCode));
        var stored = Store.GetAll();
        Assert.Equal(count, stored.Count);
        Assert.Equal(
            Enumerable.Range(0, count).Select(i => $"Hotel {i}").Order(),
            stored.Select(a => a.Name).Order());
        Assert.All(stored, a => Assert.Equal($"amenity-{a.Name[6..]}", Assert.Single(a.Amenities)));
    }

    [Fact]
    public void Store_keeps_a_copy_of_the_submitted_accommodation()
    {
        var store = new InMemoryAccommodationStore();
        var accommodation = new Accommodation
        {
            Name = "Original",
            Location = new Location { City = "Cairo", Country = "EG" },
            Amenities = ["wifi"]
        };

        store.Add(accommodation);
        accommodation.Name = "Changed";
        accommodation.Location.City = "Changed";
        accommodation.Amenities.Add("Changed");

        var stored = Assert.Single(store.GetAll());
        Assert.Equal("Original", stored.Name);
        Assert.Equal("Cairo", stored.Location.City);
        Assert.Equal(["wifi"], stored.Amenities);
    }
}
