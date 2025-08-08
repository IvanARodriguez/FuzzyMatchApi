using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FuzzyMatchApi.Infrastructure.Data;

public class CsvLocationRepository(
    ICsvParser csvParser,
    IConfiguration configuration,
    ILogger logger,
    List<LocationRecord> locations
) : ILocationRepository
{
    public Task<int> GetAddressCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LocationRecord>> GetAllAddressesAsync()
    {
        throw new NotImplementedException();
    }

    public Task LoadAddressDataAsync()
    {
        throw new NotImplementedException();
    }
}
