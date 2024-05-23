using Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Unit.Fixture;
using Xunit;

namespace Unit.Infra.Config;

public class IdentityIntelligencePublisherConfigurationHandleMessageLogAsyncTests
{
    private readonly MockLogger<IdentityIntelligencePublisherConfigurationHandleMessageLogAsyncTests> _logger;

    private readonly PubSubHelperConfigurationFixture _pubSubConfigurationFixture;

    public IdentityIntelligencePublisherConfigurationHandleMessageLogAsyncTests()
    {
        _logger = new MockLogger<IdentityIntelligencePublisherConfigurationHandleMessageLogAsyncTests>();
        Mock<ILoggerFactory> loggerFactory = new();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_logger);
        _pubSubConfigurationFixture = new PubSubHelperConfigurationFixture();
    }

    [Fact]
    public async Task HandleMessageLogAsync_ShouldFinish_WhenValidParametersProvided()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForIdentityIntelligence();
        var message = new IdentityIntelligencePublisherMessage();
        const string logMessage = "";
        const LogLevel logLevel = LogLevel.Information;

        // Act
        await subscriberConfiguration.HandleMessageLogAsync(_logger, logLevel, message, logMessage);

        var loggedMessages = _logger.GetAllMessages();

        // Assert
        loggedMessages.Should().NotBeNullOrEmpty();
        loggedMessages.Should().Be($"{logMessage} - Message contents: {message}");
    }
}
