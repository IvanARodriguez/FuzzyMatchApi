
namespace FuzzyMatchApi.Core.Models;

public record HealthResponse(
    bool IsHealthy,
    int LocationCount,
    string Status,
    DateTime Timestamp
);