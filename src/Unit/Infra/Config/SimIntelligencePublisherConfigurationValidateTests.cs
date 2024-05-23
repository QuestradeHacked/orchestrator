using Unit.Fixture;
using Xunit;

namespace Unit.Infra.Config;

public class SimIntelligencePublisherConfigurationValidateTests
{
    private readonly PubSubHelperConfigurationFixture _pubSubConfigurationFixture;

    public SimIntelligencePublisherConfigurationValidateTests()
    {
        _pubSubConfigurationFixture = new PubSubHelperConfigurationFixture();
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenProjectIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForSimIntelligence();
        subscriberConfiguration.ProjectId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenSubscriptionIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForSimIntelligence();
        subscriberConfiguration.TopicId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldNotThrowException_WhenConfigurationIsValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForSimIntelligence();

        //Act
        var exception = Record.Exception(() => subscriberConfiguration.Validate());

        // Assert
        Assert.Null(exception);
    }
}
