using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;

namespace FuzzySearchApi.Core.Services;

public class FuzzyMatchService(ILocationRepository locationRepository) : IFuzzyMatchService
{
  public double CalculateStringSimilarity(string source, string target)
  {
    throw new NotImplementedException();
  }

  public Task<MatchResult?> FindBestMatchAsync(LocationSearchRequest request)
  {
    throw new NotImplementedException();
  }
}