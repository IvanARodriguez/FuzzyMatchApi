using FuzzyMatchApi.Infrastructure.Data;
using FuzzyMatchApi.Tests.TestData;
using Microsoft.Extensions.Logging;
using Moq;

namespace FuzzyMatchApi.Tests.Unit;

public class CsvParserTests
{
    private readonly Mock<ILogger<CsvParser>> _mockLogger;
    private readonly CsvParser _parser;

    public CsvParserTests()
    {
        _mockLogger = new Mock<ILogger<CsvParser>>();
        _parser = new CsvParser(_mockLogger.Object);
    }

    [Fact]
    public async Task ParseCsvFileAsync_WhenValidCsv_ShouldReturnRecords()
    {
        // Arrange
        var testCsvPath = Path.GetTempFileName();
        await File.WriteAllTextAsync(testCsvPath, TestDataHelper.GetTestCsvContent());
        try
        {
            // Act
            var result = await _parser.ParseCsvFileAsync(testCsvPath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(5, result.Count());
            var firstLocation = result.First();
            Assert.Equal("L301", firstLocation.Code);
            Assert.Equal("123 Main Street", firstLocation.Street);
            Assert.Equal("TRIPPLE A TRANS LLC", firstLocation.LocationName);
        }
        finally
        {
            File.Delete(testCsvPath);
        }
    }

    [Fact]
    public async Task ParseCsvFileAsync_WhenFileNotExist_ShouldThrowException()
    {
        // Arrange
        var nonExistentPath = "non_existent_file.csv";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _parser.ParseCsvFileAsync(nonExistentPath)
        );
    }

    [Fact]
    public async Task ParseCsvFileAsync_WhenEmptyFile_ShouldReturnEmptyList()
    {
        // Arrange
        var testCsvPath = Path.GetTempFileName();
        await File.WriteAllTextAsync(testCsvPath, "");

        try
        {
            // Act
            var result = await _parser.ParseCsvFileAsync(testCsvPath);

            // Assert
            Assert.Empty(result);
        }
        finally
        {
            File.Delete(testCsvPath);
        }
    }

    [Fact]
    public async Task ParseCsvFileAsync_WhenMalformedLine_ShouldSkipAndContinue()
    {
        // Arrange
        var malFormedCsv = @"code,street,citystate,zip,location_name
        L301,""123 Main Street"",""New York"",""NY"",""10001"",""TRIPPLE A TRANS LLC""
        INVALID_LINE_WITH_MISSING_FIELDS
        B138,""9726 Rose Street"",""Fresno"",""CO"",""68720"",""EASTERN TRANSPORT""";

        var testCsvPath = Path.GetTempFileName();
        await File.WriteAllTextAsync(testCsvPath, malFormedCsv);

        try
        {
            // Act
            var result = await _parser.ParseCsvFileAsync(testCsvPath);
            var resultCodes = result.Select(location => location.Code).ToList();
            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains("L301", resultCodes);
            Assert.Contains("B138", resultCodes);
        }
        finally
        {
            File.Delete(testCsvPath);
        }
    }
}
