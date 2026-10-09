using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DiscoveryService.Backend.Tests;

public sealed class OpenApiContractTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OpenApiContractTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private async Task<JsonElement> GetDocument()
    {
        using var client = _factory.CreateClient();
        var json = await client.GetStringAsync("/swagger/v1/swagger.json");
        return JsonDocument.Parse(json).RootElement.Clone();
    }

    private static string[] Required(JsonElement schema) =>
        schema.GetProperty("required").EnumerateArray().Select(e => e.GetString()!).Order().ToArray();

    [Fact]
    public async Task Provide_accommodation_operation_references_the_accommodation_schema()
    {
        var doc = await GetDocument();

        var operation = doc.GetProperty("paths").GetProperty("/accommodations").GetProperty("post");
        Assert.Equal("ProvideAccommodation", operation.GetProperty("operationId").GetString());

        var body = operation.GetProperty("requestBody");
        Assert.True(body.GetProperty("required").GetBoolean());
        var content = body.GetProperty("content");
        Assert.Equal(["application/json"], content.EnumerateObject().Select(p => p.Name));
        Assert.Equal(
            "#/components/schemas/Accommodation",
            content.GetProperty("application/json").GetProperty("schema").GetProperty("$ref").GetString());

        var responses = operation.GetProperty("responses");
        Assert.True(responses.TryGetProperty("204", out _));
        Assert.True(responses.TryGetProperty("400", out _));
    }

    [Fact]
    public async Task Accommodation_and_location_schemas_declare_their_required_fields()
    {
        var schemas = (await GetDocument()).GetProperty("components").GetProperty("schemas");

        Assert.Equal(["amenities", "location", "name"], Required(schemas.GetProperty("Accommodation")));
        Assert.Equal(["city", "country"], Required(schemas.GetProperty("Location")));
    }
}
