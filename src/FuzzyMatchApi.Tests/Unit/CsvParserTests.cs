using FuzzyMatchApi.Infrastructure.Data;
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


}
