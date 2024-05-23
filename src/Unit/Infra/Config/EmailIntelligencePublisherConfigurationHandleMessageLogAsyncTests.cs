using Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Unit.Fixture;
using Xunit;

namespace Unit.Infra.Config;

public class EmailIntelligencePublisherConfigurationHandleMessageLogAsyncTests
{
    private readonly MockLogger<EmailIntelligencePublisherConfigurationHandleMessageLogAsyncTests> _logger;

    private readonly PubSubHelperConfigurationFixture _pubSubConfigurationFixture;

    public EmailIntelligencePublisherConfigurationHandleMessageLogAsyncTests()
    {
        _logger = new MockLogger<EmailIntelligencePublisherConfigurationHandleMessageLogAsyncTests>();
        Mock<ILoggerFactory> loggerFactory = new();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_logger);
        _pubSubConfigurationFixture = new PubSubHelperConfigurationFixture();
    }

    [Fact]
    public async Task HandleMessageLogAsync_ShouldFinish_WhenValidParametersProvided()
    {
        // Arrange
        var subscriberConfiguration = _pubSubConfigurationFixture.GenerateValidPublisherConfigurationForEmailIntelligence();
        var message = new EmailIntelligencePublisherMessage();
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
