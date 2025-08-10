using FuzzyMatchApi.Infrastructure.Data;
using FuzzyMatchApi.Tests.TestData;
using FuzzySearchApi.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FuzzyMatchApi.Tests.Unit;

public class CsvLocationRepositoryTests
{
  // Mocks
  private readonly Mock<ICsvParser> _mockCsvParser;
  private readonly Mock<IConfiguration> _mockConfiguration;
  private readonly Mock<ILogger<CsvLocationRepository>> _mockLogger;
  private readonly CsvLocationRepository _repository;

  public CsvLocationRepositoryTests()
  {
    _mockCsvParser = new();
    _mockConfiguration = new();
    _mockLogger = new();
    _repository = new(
      _mockCsvParser.Object,
      _mockConfiguration.Object,
      _mockLogger.Object);
  }

  [Fact]
  public async Task GetAllAddressesAsync_WhenDataNotLoaded_ShouldLoadFirst()
  {
    // Arrange
    var testData = TestDataHelper.GetTestLocationRecords();
    _mockConfiguration.Setup(config => config["CsvFilePath"])
      .Returns("locations.csv");
    _mockCsvParser.Setup(x => x.ParseCsvFileAsync(It.IsAny<string>()))
      .ReturnsAsync(testData);

    // Act
    var result = await _repository.GetAllAddressesAsync();

    // Assert
    Assert.Equal(testData.Count, result.Count());
  }

  [Fact]
  public async Task GetAddressCountAsync_ShouldReturnCorrectCount()
  {
    // Arrange
    var testData = TestDataHelper.GetTestLocationRecords();
    _mockConfiguration.Setup(config => config["CsvFilePath"])
      .Returns("locations.csv");
    _mockCsvParser.Setup(x => x.ParseCsvFileAsync(It.IsAny<string>()))
      .ReturnsAsync(testData);

    // Act
    var count = await _repository.GetAddressCountAsync();

    // Assert
    Assert.Equal(testData.Count, count);
  }

  [Fact]
  public async Task LoadAddressDataAsync_WhenCalledMultipleTimes_ShouldLoadOnlyOnce()
  {
    // Arrange
    var testData = TestDataHelper.GetTestLocationRecords();
    _mockConfiguration.Setup(config => config["CsvFilePath"])
      .Returns("locations.csv");

    _mockCsvParser.Setup(x => x.ParseCsvFileAsync(It.IsAny<string>()))
      .ReturnsAsync(testData);

    // Act
    await _repository.LoadAddressDataAsync();
    await _repository.LoadAddressDataAsync();
    await _repository.LoadAddressDataAsync();

    // Assert
    _mockCsvParser.Verify(x => x.ParseCsvFileAsync(It.IsAny<string>()), Times.Once);
  }
}