using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Casefile.Api.Middleware;
using Casefile.Domain;
using FluentAssertions;

namespace Casefile.Api.Tests;

public class CasesApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public CasesApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add(ApiKeyMiddleware.HeaderName, ApiFactory.TestApiKey);
    }

    [Fact]
    public async Task Health_is_public()
    {
        var anonymous = _factory.CreateClient();
        var response = await anonymous.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_without_api_key_is_401()
    {
        var anonymous = _factory.CreateClient();
        var response = await anonymous.PostAsJsonAsync("/api/cases", ValidBody("Need access to shared drive"));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_get_and_list_round_trip()
    {
        var created = await CreateCase("Cannot print badges", "Queue is jammed");
        created.GetProperty("id").GetGuid().Should().NotBeEmpty();
        created.GetProperty("status").GetString().Should().Be("Open");

        var id = created.GetProperty("id").GetGuid();
        var get = await _client.GetAsync($"/api/cases/{id}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await _client.GetFromJsonAsync<JsonElement>("/api/cases?page=1&pageSize=20");
        list.GetProperty("totalCount").GetInt32().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Invalid_title_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/cases", new
        {
            title = "ab",
            description = "too short",
            requesterEmail = "pat@example.com",
            severity = "Low"
        });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Cannot_skip_from_Open_to_Resolved()
    {
        var created = await CreateCase("Laptop will not boot", "black screen");
        var id = created.GetProperty("id").GetGuid();
        var response = await _client.PostAsJsonAsync($"/api/cases/{id}/status", new { status = "Resolved" });
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Can_move_Open_to_InProgress_then_Resolved()
    {
        var created = await CreateCase("VPN drops every hour", "happens on wifi");
        var id = created.GetProperty("id").GetGuid();

        var inProgress = await _client.PostAsJsonAsync($"/api/cases/{id}/status", new { status = "InProgress" });
        inProgress.StatusCode.Should().Be(HttpStatusCode.OK);

        var resolved = await _client.PostAsJsonAsync($"/api/cases/{id}/status", new { status = "Resolved" });
        resolved.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resolved.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("status").GetString().Should().Be("Resolved");
    }

    [Fact]
    public async Task Delete_only_works_while_Open()
    {
        var created = await CreateCase("Duplicate ticket", "opened twice");
        var id = created.GetProperty("id").GetGuid();

        (await _client.DeleteAsync($"/api/cases/{id}")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await _client.GetAsync($"/api/cases/{id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<JsonElement> CreateCase(string title, string description)
    {
        var response = await _client.PostAsJsonAsync("/api/cases", ValidBody(title, description));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    private static object ValidBody(string title, string description = "details") => new
    {
        title,
        description,
        requesterEmail = "pat@example.com",
        severity = nameof(CaseSeverity.Medium)
    };
}
