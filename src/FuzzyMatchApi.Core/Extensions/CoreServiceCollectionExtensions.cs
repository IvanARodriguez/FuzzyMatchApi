using FuzzyMatchApi.Core.Interfaces;
using FuzzyMatchApi.Core.Services;
using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FuzzyMatchApi.Core.Extensions;

public static class CoreServiceCollectionExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<IFuzzyMatchService, FuzzyMatchService>();
    services.AddScoped<IStringSimilarityService, StringSimilarityService>();
    return services;
  }
}