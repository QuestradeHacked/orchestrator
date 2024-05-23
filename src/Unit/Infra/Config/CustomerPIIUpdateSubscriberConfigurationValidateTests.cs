using Unit.Fixture;
using Xunit;

namespace Unit.Infra.Config;

public class CustomerPiiUpdateSubscriberConfigurationValidateTests
{
    private readonly PubSubHelperConfigurationFixture _pubSubHelperConfigurationFixture = new();

    [Fact]
    public void Validate_ShouldThrowException_WhenProjectIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubHelperConfigurationFixture.GenerateValidPiiSubscriberConfiguration();
        subscriberConfiguration.ProjectId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenSubscriptionIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubHelperConfigurationFixture.GenerateValidPiiSubscriberConfiguration();
        subscriberConfiguration.SubscriptionId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldNotThrowException_WhenConfigurationIsValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubHelperConfigurationFixture.GenerateValidPiiSubscriberConfiguration();

        //Act
        var exception = Record.Exception(() => subscriberConfiguration.Validate());

        // Assert
        Assert.Null(exception);
    }
}
