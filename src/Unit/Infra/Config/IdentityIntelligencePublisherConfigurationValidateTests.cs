using Unit.Fixture;
using Xunit;

namespace Unit.Infra.Config;

public class IdentityIntelligencePublisherConfigurationValidateTests
{
    private readonly PubSubHelperConfigurationFixture _pubSubConfigurationFixture = new();

    [Fact]
    public void Validate_ShouldThrowException_WhenProjectIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForIdentityIntelligence();
        subscriberConfiguration.ProjectId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenSubscriptionIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForIdentityIntelligence();
        subscriberConfiguration.TopicId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldNotThrowException_WhenConfigurationIsValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForIdentityIntelligence();

        //Act
        var exception = Record.Exception(() => subscriberConfiguration.Validate());

        // Assert
        Assert.Null(exception);
    }
}
