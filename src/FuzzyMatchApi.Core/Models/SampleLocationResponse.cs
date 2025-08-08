
using FuzzySearchApi.Core.Models;

namespace FuzzyMatchApi.Core.Models;

public record SampleLocationResponse(IEnumerable<LocationRecord> Locations, int SampleSize, int TotalCount);
