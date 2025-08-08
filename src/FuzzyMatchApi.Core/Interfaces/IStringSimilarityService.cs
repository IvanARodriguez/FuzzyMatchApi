namespace FuzzyMatchApi.Core.Interfaces;

public interface IStringSimilarityService
{
  double CalculateLevenshteinSimilarity(string source, string target);
  double CalculateJaroWinklerSimilarity(string source, string target);
  double CalculateTokenSetRatio(string source, string target);
  double CalculateWeightedSimilarity(string source, string target);
}
