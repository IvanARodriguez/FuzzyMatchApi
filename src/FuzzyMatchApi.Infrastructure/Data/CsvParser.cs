using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;

namespace FuzzyMatchApi.Infrastructure.Data;

public class CsvParser : ICsvParser
{
    public Task<IEnumerable<LocationRecord>> ParseCsvFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }
}
