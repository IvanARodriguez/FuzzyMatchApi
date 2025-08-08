using FuzzySearchApi.Core.Models;

namespace FuzzySearchApi.Core.Interfaces;

public interface ICsvParser
{
  Task<IEnumerable<LocationRecord>> ParseCsvFileAsync(string filePath);
}