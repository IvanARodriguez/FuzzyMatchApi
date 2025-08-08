using System.Text.RegularExpressions;
using FuzzyMatchApi.Core.Interfaces;
using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;

namespace FuzzySearchApi.Core.Services;

public class FuzzyMatchService(ILocationRepository locationRepository, IStringSimilarityService similarityService) : IFuzzyMatchService
{
  public async Task<MatchResult?> FindBestMatchAsync(LocationSearchRequest request)
  {
    var locations = await locationRepository.GetAllAddressesAsync();

    if (!locations.Any()) return null;

    var bestMatch = locations
      .Select(record => CalculateMatchScore(request, record))
      .OrderByDescending(Match => Match.MatchRate)
      .FirstOrDefault();

    return bestMatch?.MatchRate > 0.1 ? bestMatch : null;
  }

  public double CalculateStringSimilarity(string source, string target)
  {
    return similarityService.CalculateWeightedSimilarity(source, target);
  }

  private MatchResult CalculateMatchScore(LocationSearchRequest request, LocationRecord record)
  {
    var fieldScores = new Dictionary<string, double>();
    var scores = new List<double>();
    var weights = new List<double>();

    if (!string.IsNullOrWhiteSpace(request.Company))
    {
      var companyScore = similarityService.CalculateWeightedSimilarity(
          request.Company, record.LocationName);
      fieldScores["Company"] = Math.Round(companyScore, 4);
      scores.Add(companyScore);
      weights.Add(0.35);
    }

    if (!string.IsNullOrWhiteSpace(request.Street))
    {
      var streetScore = similarityService.CalculateWeightedSimilarity(request.Street, record.Street);
      fieldScores["Street"] = Math.Round(streetScore, 4);
      scores.Add(streetScore);
      weights.Add(0.40);
    }

    if (!string.IsNullOrWhiteSpace(request.City))
    {
      var cityScore = similarityService.CalculateWeightedSimilarity(request.City, record.City);
      fieldScores["City"] = Math.Round(cityScore, 4);
      scores.Add(cityScore);
      weights.Add(0.20);
    }

    if (!string.IsNullOrWhiteSpace(request.State))
    {
      var stateScore = CalculateStateScore(request.State, record.State);
      fieldScores["State"] = Math.Round(stateScore, 4);
      scores.Add(stateScore);
      weights.Add(0.05);
    }

    // calculated weighted average
    var totalWeight = weights.Sum();
    var weightedSum = scores.Zip(weights, (score, weight) => score * weight).Sum();
    var matchRate = totalWeight > 0 ? weightedSum / totalWeight : 0;

    return new MatchResult(record, Math.Round(matchRate, 4), fieldScores);
  }

  private double CalculateStateScore(string sourceState, string targetState)
  {
    if (string.IsNullOrEmpty(sourceState) || string.IsNullOrEmpty(targetState)) return 0;

    if (string.Equals(sourceState.Trim(), targetState.Trim(), StringComparison.OrdinalIgnoreCase)) return 1.0;

    return similarityService.CalculateLevenshteinSimilarity(sourceState, targetState) * 0.8;
  }
}