using FuzzyMatchApi.Core.Services;

namespace FuzzyMatchApi.Tests.Unit;

public class StringSimilarityServiceTests
{
    private readonly StringSimilarityService _service;
    public StringSimilarityServiceTests()
    {
        _service = new StringSimilarityService();
    }

    [Theory]
    [InlineData("EASTERN TRANSPORT", "EASTERN TRANSPORT", 1.0)]
    [InlineData("EASTERN TRANSPORT", "eastern transport", 1.0)]
    [InlineData("EASTERN TRANSPORT", "EASTERN TRANS", 0.85)]
    [InlineData("TRIPPLE A TRANS LLC", "TRIPLE A TRANSPORT", 0.75)]
    [InlineData("123 Main Street", "123 Main St", 0.75)]
    [InlineData("", "", 0.0)]
    [InlineData("Test", "", 0.0)]
    [InlineData("", "Test", 0.0)]
    public void CalculateWeightedSimilarity_ShouldReturnExpectedScores(
        string source, string target, double expectedMinScore)
    {
        // Act
        var result = _service.CalculateWeightedSimilarity(source, target);

        // Assert
        Assert.True(result >= expectedMinScore,
                 $"Expected at least {expectedMinScore}, but got {result}");
        Assert.True(result <= 1.0,
            $"Expected at most 1.0, but got {result}");
    }

    [Theory]
    [InlineData("hello", "hello", 1.0)]
    [InlineData("hello", "hallo", 0.8)]
    [InlineData("hello", "world", 0.0)]
    public void CalculateLevenshteinSimilarity_ShouldCalculateCorrectly(
    string source, string target, double expectedMinScore)
    {
        // Act
        var result = _service.CalculateLevenshteinSimilarity(source, target);

        // Assert
        Assert.True(result >= expectedMinScore,
                $"Expected at least {expectedMinScore}, but got {result}");
    }

    [Fact]
    public void CalculateTokenSetRatio_ShouldHandleWordOrderDifferences()
    {
        // Arrange
        var source = "EASTER TRANSPORT LLC";
        var target = "LLC TRANSPORT EASTERN";

        // Act
        var result = _service.CalculateTokenSetRatio(source, target);

        // Assert
        Assert.True(result > 0.8, $"Expected to be greater than 0.8, but got {result}");
    }


}
