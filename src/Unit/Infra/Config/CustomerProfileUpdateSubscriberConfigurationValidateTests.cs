using Unit.Fixture;
using Xunit;

namespace Unit.Infra.Config;

public class CustomerProfileUpdateSubscriberConfigurationValidateTests
{
    private readonly PubSubHelperConfigurationFixture _pubSubHelperConfigurationFixture;

    public CustomerProfileUpdateSubscriberConfigurationValidateTests()
    {
        _pubSubHelperConfigurationFixture = new PubSubHelperConfigurationFixture();
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenProjectIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubHelperConfigurationFixture.GenerateValidSubscriberConfiguration();
        subscriberConfiguration.ProjectId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenSubscriptionIdIsNotValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubHelperConfigurationFixture.GenerateValidSubscriberConfiguration();
        subscriberConfiguration.SubscriptionId = " ";

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            subscriberConfiguration.Validate());
    }

    [Fact]
    public void Validate_ShouldNotThrowException_WhenConfigurationIsValid()
    {
        // Arrange
        var subscriberConfiguration = _pubSubHelperConfigurationFixture.GenerateValidSubscriberConfiguration();

        //Act
        var exception = Record.Exception(() => subscriberConfiguration.Validate());

        // Assert
        Assert.Null(exception);
    }
}
