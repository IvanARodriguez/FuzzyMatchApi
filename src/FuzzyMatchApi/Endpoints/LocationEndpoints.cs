using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FuzzyMatchApi.Endpoints;

public static class LocationEndpoints
{
    public static void MapLocationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/address")
            .WithTags("Location Matching")
            .WithOpenApi();

        group.MapPost("/search", SearchAddress);
        // group.MapGet("/count", GetAddressCount);
        // group.MapGet("/sample", GetSampleAddresses);
    }

    private static async Task<IResult> SearchAddress(
        [FromBody] LocationSearchRequest request,
        IFuzzyMatchService fuzzyMatchService)
    {
        // implementation
        return Results.Ok();
    }
}
