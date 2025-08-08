namespace FuzzySearchApi.Core.Models;

public record MatchResult(
  LocationRecord Record,
  double MatchRate,
  Dictionary<string, double> FieldScores
);