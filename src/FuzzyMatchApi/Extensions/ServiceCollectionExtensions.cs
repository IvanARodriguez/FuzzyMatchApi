using FuzzyMatchApi.Core.Interfaces;
using FuzzyMatchApi.Core.Services;
using FuzzyMatchApi.Infrastructure.Data;
using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Services;

namespace FuzzyMatchApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
        });

        return services;
    }
}