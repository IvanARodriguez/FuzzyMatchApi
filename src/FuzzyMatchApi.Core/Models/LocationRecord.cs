namespace FuzzySearchApi.Core.Models;

public record LocationRecord(
  string Code,
  string Street,
  string City,
  string State,
  string Zip,
  string LocationName
);