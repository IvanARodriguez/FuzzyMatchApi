using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;
using Microsoft.Extensions.Logging;

namespace FuzzyMatchApi.Infrastructure.Data;

public class CsvParser(ILogger<CsvParser> logger) : ICsvParser
{
    public async Task<IEnumerable<LocationRecord>> ParseCsvFileAsync(string filePath)
    {
        var records = new List<LocationRecord>();
        try
        {
            var lines = await File.ReadAllLinesAsync(filePath);

            if (lines.Length == 0)
            {
                logger.LogWarning("CSV file is empty: {FilePath}", filePath);
                return records;
            }

            // Skip header line
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var record = ParseCsvLine(lines[i]);
                    if (record != null)
                        records.Add(record);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to parse line {LineNumber}: {Line}", i + 1, lines[i]);
                }
            }

            logger.LogInformation("Successfully parsed {Count} records from {FilePath}",
                records.Count, filePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading CSV file: {FilePath}", filePath);
            throw;
        }

        return records;
    }

    private LocationRecord? ParseCsvLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var fields = ParseCsvFields(line);

        if (fields.Count < 6)
        {
            logger.LogWarning("Line has insufficient fields: {Line}", line);
            return null;
        }

        return new LocationRecord(
            Code: fields[0].Trim(),
            Street: fields[1].Trim(),
            City: fields[2].Trim(),
            State: fields[3].Trim(),
            Zip: fields[4].Trim(),
            LocationName: fields[5].Trim()
        );
    }

    private List<string> ParseCsvFields(string line)
    {
        var fields = new List<string>();
        var current = "";
        var inQuotes = false;
        var i = 0;

        while (i < line.Length)
        {
            var c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Escaped quote
                    current += '"';
                    i += 2;
                }
                else
                {
                    // Toggle quote state
                    inQuotes = !inQuotes;
                    i++;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // Field separator
                fields.Add(current);
                current = "";
                i++;
            }
            else
            {
                current += c;
                i++;
            }
        }

        // Add the last field
        fields.Add(current);

        return fields;
    }
}
