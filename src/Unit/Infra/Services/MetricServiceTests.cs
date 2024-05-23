using Infra.Services;
using NSubstitute;
using StatsdClient;
using Xunit;

namespace Unit.Infra.Services;

public class MetricServiceTests
{
    private readonly IDogStatsd _dogStatsd;
    private readonly MetricService _service;
    private const string MetricName = "sampleMetric";

    public MetricServiceTests()
    {
        _dogStatsd = Substitute.For<IDogStatsd>();
        _service = new MetricService(_dogStatsd);
    }

    [Fact]
    public void Distribution_ShouldCallDogStatsdClient()
    {
        //Arrange
        const double latency = 101;

        // Act
        _service.Distribution(MetricName, latency, new List<string>());

        // Assert
        _dogStatsd.Received(1)
            .Distribution(Arg.Is(MetricName), Arg.Is(latency), tags: Arg.Any<string[]>());
    }

    [Fact]
    public void Increment_ShouldCallDogStatsdClient()
    {
        // Act
        _service.Increment(MetricName, new List<string>());

        // Assert
        _dogStatsd.Received(1)
            .Increment(Arg.Is(MetricName), tags: Arg.Any<string[]>());
    }

    [Fact]
    public void StartTimer_ShouldCallDogStatsdClient()
    {
        // Act
        _service.StartTimer(MetricName, new List<string>());

        // Assert
        _dogStatsd.Received(1)
            .StartTimer(Arg.Is(MetricName), tags: Arg.Any<string[]>());
    }
}
