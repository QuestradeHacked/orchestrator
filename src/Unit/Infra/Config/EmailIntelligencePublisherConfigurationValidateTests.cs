using Unit.Fixture;
using Xunit;

namespace Unit.Infra.Config;

public class EmailIntelligencePublisherConfigurationValidateTests
{
    private readonly PubSubHelperConfigurationFixture _pubSubConfigurationFixture;

    public EmailIntelligencePublisherConfigurationValidateTests()
    {
        _pubSubConfigurationFixture = new PubSubHelperConfigurationFixture();
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenProjectIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForEmailIntelligence();
        subscriberConfiguration.ProjectId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenSubscriptionIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForEmailIntelligence();
        subscriberConfiguration.TopicId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldNotThrowException_WhenConfigurationIsValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForEmailIntelligence();

        //Act
        var exception = Record.Exception(() => subscriberConfiguration.Validate());

        // Assert
        Assert.Null(exception);
    }
}
