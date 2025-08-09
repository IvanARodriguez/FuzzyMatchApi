using FuzzyMatchApi.Core.Interfaces;
using FuzzyMatchApi.Tests.TestData;
using FuzzySearchApi.Core.Interfaces;
using FuzzySearchApi.Core.Models;
using FuzzySearchApi.Core.Services;
using Moq;

namespace FuzzyMatchApi.Tests.Unit;

public class FuzzyMatchServiceTests
{
    private readonly Mock<ILocationRepository> _mockRepository;
    private readonly Mock<IStringSimilarityService> _mockSimilarityService;
    private readonly FuzzyMatchService _service;

    public FuzzyMatchServiceTests()
    {
        _mockRepository = new Mock<ILocationRepository>();
        _mockSimilarityService = new Mock<IStringSimilarityService>();

        _service = new FuzzyMatchService(
            _mockRepository.Object,
            _mockSimilarityService.Object
        );
    }

    [Fact]
    public async Task FindBestMatchAsync_WhenNoAddresses_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAddressesAsync()).ReturnsAsync(new List<LocationRecord>());

        var request = TestDataHelper.CreateSearchRequest(company: "Test");

        // Act
        var result = await _service.FindBestMatchAsync(request);

        //Assert
        Assert.True(result is null, $"Expected result to be null, but got {result}");
    }

    [Fact]
    public async Task FindBestMatchAsync_WhenExactMatch_ShouldReturnHighScore()
    {
        // Arrange
        var testData = TestDataHelper.GetTestLocationRecords();
        _mockRepository.Setup(x => x.GetAllAddressesAsync())
            .ReturnsAsync(testData);

        _mockSimilarityService.Setup(x => x.CalculateWeightedSimilarity(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(1.0);

        var request = TestDataHelper.CreateSearchRequest(
            company: "EASTERN TRANSPORT",
            street: "9726 Rose Street",
            city: "Fresno",
            state: "CO"
        );

        // Act
        var result = await _service.FindBestMatchAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("B138", result!.Record.Code);
        Assert.True(result.MatchRate > 0.9, $"Expected MatchRate > 0.9 but got {result.MatchRate}");
    }

    [Fact]
    public async Task FindBestMatchAsync_WhenCompanyOnly_ShouldMatchByCompany()
    {
        // Arrange
        var testData = TestDataHelper.GetTestLocationRecords();
        _mockRepository.Setup(x => x.GetAllAddressesAsync())
            .ReturnsAsync(testData);

        _mockSimilarityService.Setup(x => x.CalculateWeightedSimilarity("EASTERN TRANSPORT", "EASTERN TRANSPORT"))
            .Returns(1.0);

        _mockSimilarityService.Setup(x => x.CalculateWeightedSimilarity("EASTERN TRANSPORT", It.IsNotIn("EASTERN TRANSPORT")))
            .Returns(0.3);

        var request = TestDataHelper.CreateSearchRequest(company: "EASTERN TRANSPORT");

        // Act
        var result = await _service.FindBestMatchAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("EASTERN TRANSPORT", result!.Record.LocationName);
        Assert.True(result.FieldScores.ContainsKey("Company"), "Expected 'Company' key to exist in FieldScores");

    }

}
