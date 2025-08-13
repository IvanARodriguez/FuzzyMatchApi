using System.Net;
using System.Text.Json;
using FuzzyMatchApi.Core.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FuzzyMatchApi.Tests.Integration;

public class LocationEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly WebApplicationFactory<Program> _factory;
  private readonly HttpClient _client;

  public LocationEndpointTests(WebApplicationFactory<Program> factory)
  {
    _factory = factory;
    _client = _factory.CreateClient();
  }

  [Fact]
  public async Task GetHealth_ShouldReturnOk()
  {
    // Act
    var response = await _client.GetAsync("/api/address/health");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    var healthResponse = JsonSerializer.Deserialize<HealthResponse>(content, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });

    Assert.NotNull(healthResponse);
    var now = DateTime.UtcNow;
    Assert.InRange(
        healthResponse!.Timestamp,
        now - TimeSpan.FromMinutes(1),
        now + TimeSpan.FromMinutes(1)
    );
  }

  [Fact]
  public async Task GetCount_ShouldReturnLocationCount()
  {
    // Act
    var response = await _client.GetAsync("/api/address/count");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    var countResponse = JsonSerializer.Deserialize<LocationCountResponse>(content, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });

    Assert.NotNull(countResponse);
    Assert.True(countResponse.Count >= 0);
  }

  [Fact]
  public async Task GetSample_ShouldReturnSampleLocation()
  {
    // Act
    var response = await _client.GetAsync("/api/address/sample");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    var sampleResponse = JsonSerializer.Deserialize<SampleLocationResponse>(content, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });

    Assert.NotNull(sampleResponse);
    Assert.Equal(5, sampleResponse.Locations.Count());
  }

  [Fact]
  public async Task GetSample_WithCustomCount_ShouldReturnCorrectNumber()
  {
    // Act
    var response = await _client.GetAsync("/api/address/sample?count=3");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();

    var sampleResponse = JsonSerializer.Deserialize<SampleLocationResponse>(content, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });

    Assert.NotNull(sampleResponse);
    Assert.True(sampleResponse.SampleSize <= 3);
  }

}