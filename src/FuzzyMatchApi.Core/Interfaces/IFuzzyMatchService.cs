using FuzzySearchApi.Core.Models;

namespace FuzzySearchApi.Core.Interfaces;

public interface IFuzzyMatchService
{
  Task<MatchResult?> FindBestMatchAsync(LocationSearchRequest request);
  double CalculateStringSimilarity(string source, string target);
}