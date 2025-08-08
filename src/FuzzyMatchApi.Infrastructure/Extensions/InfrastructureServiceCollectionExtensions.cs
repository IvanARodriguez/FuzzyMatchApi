using FuzzyMatchApi.Infrastructure.Data;
using FuzzySearchApi.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FuzzyMatchApi.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ILocationRepository, CsvLocationRepository>();
        services.AddScoped<ICsvParser, CsvParser>();
        return services;
    }
}
