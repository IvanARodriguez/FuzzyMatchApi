using FuzzyMatchApi.Core.Models;
using FuzzyMatchApi.Logging;
using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FuzzyMatchApi.Endpoints;

public static class LocationEndpoints
{
    public static void MapLocationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/address")
            .WithTags("Location Matching")
            .WithOpenApi();

        group.MapPost("/search", SearchAddress)
            .WithName("SearchLocation")
            .WithSummary("Search for location using fuzzy matching")
            .WithDescription("Finds the best matching location based on company name, street, city, and state using fuzzy string matching algorithms")
            .Produces<MatchResult>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/count", GetAddressCount)
            .WithName("GetLocationCount")
            .WithSummary("Get total number of locations")
            .WithDescription("Returns the total count of locations loaded in the system")
            .Produces<LocationCountResponse>(StatusCodes.Status200OK);

        group.MapGet("/sample", GetSampleAddresses)
            .WithName("GetSampleLocations")
            .WithSummary("Get sample locations")
            .WithDescription("Returns a sample of locations for testing and preview purposes")
            .Produces<IEnumerable<LocationRecord>>(StatusCodes.Status200OK);

        group.MapGet("/health", GetHealthStatus)
            .WithName("GetLocationServiceHealth")
            .WithSummary("Check location service health")
            .WithDescription("Returns the health status of the location service and data loading status")
            .Produces<HealthResponse>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> SearchAddress(
        [FromBody] LocationSearchRequest request,
        IFuzzyMatchService fuzzyMatchService,
      ILogger<LocationEndpointsLoggerCategory> logger)
    {
        try
        {
            // Validate request
            var validationResult = ValidateSearchRequest(request);
            if (validationResult != null)
                return validationResult;

            logger.LogInformation("Searching for location with request: {@Request}", request);

            var result = await fuzzyMatchService.FindBestMatchAsync(request);

            if (result == null)
            {
                logger.LogInformation("No matches found for request: {@Request}", request);
                return Results.NotFound(new
                {
                    message = "No matches found",
                    searchCriteria = request
                });
            }

            logger.LogInformation("Found match with score {MatchRate} for request: {@Request}",
                result.MatchRate, request);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while searching for location: {@Request}", request);
            return Results.Problem(
                title: "Search Error",
                detail: "An error occurred while processing your search request",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> GetAddressCount(
        ILocationRepository locationRepository,
        ILogger<LocationEndpointsLoggerCategory> logger)
    {
        try
        {
            var count = await locationRepository.GetAddressCountAsync();
            logger.LogDebug("Retrieved location count: {Count}", count);

            return Results.Ok(new LocationCountResponse(
                Count: count,
                Message: count > 0 ? "Locations loaded successfully" : "No locations loaded"
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving location count");
            return Results.Problem(
                title: "Count Error",
                detail: "An error occurred while retrieving the location count",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> GetSampleAddresses(
        ILocationRepository locationRepository,
        ILogger<LocationEndpointsLoggerCategory> logger,
        [FromQuery] int count = 5)
    {
        try
        {
            // Validate count parameter
            if (count < 1 || count > 50)
            {
                return Results.BadRequest(new
                {
                    message = "Count parameter must be between 1 and 50"
                });
            }

            var allAddresses = await locationRepository.GetAllAddressesAsync();
            var sample = allAddresses.Take(count).ToList();

            logger.LogDebug("Retrieved {SampleCount} sample locations out of {TotalCount}",
                sample.Count, allAddresses.Count());

            return Results.Ok(new SampleLocationResponse(
                Locations: sample,
                SampleSize: sample.Count,
                TotalCount: allAddresses.Count()
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving sample locations");
            return Results.Problem(
                title: "Sample Error",
                detail: "An error occurred while retrieving sample locations",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> GetHealthStatus(
        ILocationRepository locationRepository,
        ILogger<LocationEndpointsLoggerCategory> logger)
    {
        try
        {
            var count = await locationRepository.GetAddressCountAsync();
            var isHealthy = count > 0;

            var health = new HealthResponse(
                IsHealthy: isHealthy,
                LocationCount: count,
                Status: isHealthy ? "Healthy" : "No data loaded",
                Timestamp: DateTime.UtcNow
            );

            logger.LogDebug("Health check completed: {@Health}", health);

            return isHealthy
                ? Results.Ok(health)
                : Results.Ok(health); // Still return 200 OK but with unhealthy status
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during health check");

            var unhealthyResponse = new HealthResponse(
                IsHealthy: false,
                LocationCount: 0,
                Status: "Error: " + ex.Message,
                Timestamp: DateTime.UtcNow
            );

            return Results.Ok(unhealthyResponse);
        }
    }

    private static IResult? ValidateSearchRequest(LocationSearchRequest request)
    {
        var errors = new List<string>();

        // Check if at least one search parameter is provided
        if (string.IsNullOrWhiteSpace(request.Company) &&
            string.IsNullOrWhiteSpace(request.Street) &&
            string.IsNullOrWhiteSpace(request.City) &&
            string.IsNullOrWhiteSpace(request.State))
        {
            errors.Add("At least one search parameter (Company, Street, City, or State) must be provided");
        }

        // Validate individual fields
        if (!string.IsNullOrWhiteSpace(request.Company) && request.Company.Length > 200)
        {
            errors.Add("Company name cannot exceed 200 characters");
        }

        if (!string.IsNullOrWhiteSpace(request.Street) && request.Street.Length > 200)
        {
            errors.Add("Street address cannot exceed 200 characters");
        }

        if (!string.IsNullOrWhiteSpace(request.City) && request.City.Length > 100)
        {
            errors.Add("City name cannot exceed 100 characters");
        }

        if (!string.IsNullOrWhiteSpace(request.State) && request.State.Length > 50)
        {
            errors.Add("State cannot exceed 50 characters");
        }

        // Validate state format if provided (should be 2-letter code or full name)
        if (!string.IsNullOrWhiteSpace(request.State))
        {
            var state = request.State.Trim();
            if (state.Length > 50)
            {
                errors.Add("Invalid state format");
            }
        }

        if (errors.Any())
        {
            return Results.ValidationProblem(errors.ToDictionary(
                error => "validation",
                error => new[] { error }));
        }

        return null;
    }
}

