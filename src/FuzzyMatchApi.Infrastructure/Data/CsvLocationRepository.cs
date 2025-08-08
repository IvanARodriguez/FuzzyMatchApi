using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FuzzyMatchApi.Infrastructure.Data;

public class CsvLocationRepository(
    ICsvParser csvParser,
    IConfiguration configuration,
    ILogger<CsvLocationRepository> logger
) : ILocationRepository
{
    private readonly List<LocationRecord> _locations = [];
    private bool _isLoaded = false;
    public async Task<int> GetAddressCountAsync()
    {
        if (!_isLoaded)
            await LoadAddressDataAsync();

        return _locations.Count;
    }

    public async Task<IEnumerable<LocationRecord>> GetAllAddressesAsync()
    {
        if (!_isLoaded)
            await LoadAddressDataAsync();

        return _locations;
    }

    public async Task LoadAddressDataAsync()
    {
        if (_isLoaded)
            return;

        try
        {
            var csvPathFromConfig = configuration["CsvFilePath"];
            var csvPath = !string.IsNullOrWhiteSpace(csvPathFromConfig)
                ? Path.Combine(AppContext.BaseDirectory, csvPathFromConfig)
                : Path.Combine(AppContext.BaseDirectory, "data", "locations.csv");
            if (!File.Exists(csvPath))
            {
                logger.LogError("CSV file not found at path: {CsvPath}", csvPath);
                return;
            }

            var records = await csvParser.ParseCsvFileAsync(csvPath);
            _locations.Clear();
            _locations.AddRange(records);

            _isLoaded = true;
            logger.LogInformation("Successfully loaded {Count} location records from {CsvPath}",
                _locations.Count, csvPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading CSV data");
            throw;
        }
    }
}
