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

}
