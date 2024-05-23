using Domain.Models.Analysis;
using Domain.Services;
using Infra.Subscriber;
using Infra.Util;
using Integration.Faker;
using Integration.Fixture;
using Integration.Providers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using Questrade.Library.PubSubClientHelper.Primitives;
using System.Text.Json;
using Xunit;

namespace Integration.Infra.Subscriber;

public class CustomerPiiUpdateSubscriberTests : IAssemblyFixture<PubSubEmulatorFixture>
{
    private readonly LoggerFixture<CustomerPiiUpdateSubscriberTests> _loggerFixture;
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly IMetricService _metricService = Substitute.For<IMetricService>();
    private readonly PubSubEmulatorFixture _pubSubFixture;
    private readonly CustomerPiiUpdateSubscriber _subscriber;
    private readonly int _subscriberTimeout;
    private readonly string _topicId;

    public CustomerPiiUpdateSubscriberTests(PubSubEmulatorFixture pubSubEmulatorFixture)
    {
        _loggerFixture = new LoggerFixture<CustomerPiiUpdateSubscriberTests>();
        Mock<ILoggerFactory> loggerFactory = new();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_loggerFixture);


        _pubSubFixture = pubSubEmulatorFixture;
        _subscriberTimeout = _pubSubFixture.SubscriberTimeout;
        _topicId = $"T_{Guid.NewGuid()}";

        var services = new ServiceCollection();
        services.AddSingleton<CorrelationContext>();
        services.AddSingleton<IDefaultJsonSerializerOptionsProvider, MyDefaultJsonSerializerOptionsProvider>();
        var serviceProvider = services.AddMemoryCache().BuildServiceProvider();

        var subscriptionId = $"{_topicId}.{Guid.NewGuid()}";
        var subscriberConfig = _pubSubFixture.CreateDefaultSubscriberPiiConfig(subscriptionId);

        _subscriber = new CustomerPiiUpdateSubscriber
        (
            loggerFactory.Object,
            subscriberConfig,
            serviceProvider,
            _metricService,
            _mediator
        );

        _pubSubFixture.CreateTopic(_topicId);
        _pubSubFixture.CreateSubscription(_topicId, subscriptionId);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldSendAllUpdates_WhenReceivesAnAddressChange()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GeneratePubSubMessage(CustomerPiiUpdateMessageFaker.GenerateUpdateMessage());
        var emailMessageExpected = $"The message {customerPiiUpdateMessage.Id} has an e-mail updated";
        var phoneMessageExpected = $"The message {customerPiiUpdateMessage.Id} has a phone number updated";

        // Act
        await publisher.PublishAsync(JsonSerializer.Serialize(customerPiiUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains(emailMessageExpected, loggedMessages);
        Assert.Contains(phoneMessageExpected, loggedMessages);

        await _mediator.Received(3).Send(Arg.Any<AnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerEmailAnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerIdentityAnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerPhoneAnalysisRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldSendUpdate_WhenReceivesAnEmailChange()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GeneratePubSubMessage(CustomerPiiUpdateMessageFaker.GenerateEmailUpdateMessage());
        var emailMessageExpected = $"The message {customerPiiUpdateMessage.Id} has an e-mail updated";


        // Act
        await publisher.PublishAsync(JsonSerializer.Serialize(customerPiiUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains(emailMessageExpected, loggedMessages);

        await _mediator.Received(1).Send(Arg.Any<AnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerEmailAnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.DidNotReceive().Send(Arg.Any<CustomerIdentityAnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.DidNotReceive().Send(Arg.Any<CustomerPhoneAnalysisRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldSendUpdate_WhenReceivesAnPhoneChange()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GeneratePubSubMessage(CustomerPiiUpdateMessageFaker.GeneratePhoneUpdateMessage());
        var phoneMessageExpected = $"The message {customerPiiUpdateMessage.Id} has a phone number updated";


        // Act
        await publisher.PublishAsync(JsonSerializer.Serialize(customerPiiUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains(phoneMessageExpected, loggedMessages);

        await _mediator.Received(2).Send(Arg.Any<AnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.DidNotReceive().Send(Arg.Any<CustomerEmailAnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerIdentityAnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerPhoneAnalysisRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldSendAllUpdates_WhenReceivesAnEmailAndPhoneChange()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerPiiUpdateMessage = CustomerPiiUpdateMessageFaker
            .GeneratePubSubMessage(CustomerPiiUpdateMessageFaker.GenerateUpdateMessageWithNoAddressChanges());
        var emailMessageExpected = $"The message {customerPiiUpdateMessage.Id} has an e-mail updated";
        var phoneMessageExpected = $"The message {customerPiiUpdateMessage.Id} has a phone number updated";

        // Act
        await publisher.PublishAsync(JsonSerializer.Serialize(customerPiiUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert

        Assert.Contains(emailMessageExpected, loggedMessages);
        Assert.Contains(phoneMessageExpected, loggedMessages);

        await _mediator.Received(3).Send(Arg.Any<AnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerEmailAnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerIdentityAnalysisRequest>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Send(Arg.Any<CustomerPhoneAnalysisRequest>(),
            Arg.Any<CancellationToken>());
    }
}
