using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.CRM;
using Domain.Repositories.Firestore;
using Domain.Repositories.GraphQL;
using Domain.Services;
using Google.Cloud.PubSub.V1;
using Infra.Constants;
using Infra.Subscriber;
using Infra.Util;
using Integration.Faker;
using Integration.Fixture;
using Integration.Providers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questrade.Library.PubSubClientHelper.Primitives;
using Xunit;

namespace Integration.Infra.Subscriber;

public class CustomerProfileUpdateSubscribeMetricsTests : IAssemblyFixture<PubSubEmulatorFixture>
{
    private readonly ICustomerOnHoldRepository _customerOnHoldRepository;

    private readonly ICustomerRepository _customerRepository;

    private readonly IMetricService _metricService;

    private readonly PubSubEmulatorFixture _pubSubFixture;

    private readonly CustomerProfileUpdateSubscriber _subscriber;

    private readonly int _subscriberTimeout;

    private readonly string _topicId;

    public CustomerProfileUpdateSubscribeMetricsTests(PubSubEmulatorFixture pubSubEmulatorFixture)
    {
        var loggerFixture = new LoggerFixture<CustomerProfileUpdateSubscriberPubSubTests>();
        var mediator = Substitute.For<IMediator>();
        _metricService = Substitute.For<IMetricService>();
        _pubSubFixture = pubSubEmulatorFixture;
        _subscriberTimeout = _pubSubFixture.SubscriberTimeout;
        _topicId = $"T_{Guid.NewGuid()}";

        _customerOnHoldRepository = Substitute.For<ICustomerOnHoldRepository>();
        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        _customerOnHoldRepository.UpsertAsync(Arg.Any<CustomerOnHold>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(string.Empty));

        _customerRepository = Substitute.For<ICustomerRepository>();
        _customerRepository
            .GetProfileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Tuple.Create<CrmPerson?,PersonAccount?>(new CrmPerson(), new PersonAccount()));

        var services = new ServiceCollection();
        services.AddSingleton<CorrelationContext>();
        services.AddSingleton<IDefaultJsonSerializerOptionsProvider, MyDefaultJsonSerializerOptionsProvider>();
        var serviceProvider = services.AddMemoryCache().BuildServiceProvider();

        var subscriptionId = $"{_topicId}.{Guid.NewGuid()}";
        var subscriberConfig = _pubSubFixture.CreateDefaultSubscriberConfig(subscriptionId);
        Mock<ILoggerFactory> loggerFactory = new();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerFixture);
        _subscriber = new CustomerProfileUpdateSubscriber(
            loggerFactory.Object,
            subscriberConfig,
            serviceProvider,
            mediator,
            _metricService,
            _customerRepository,
            _customerOnHoldRepository
        );

        _pubSubFixture.CreateTopic(_topicId);
        _pubSubFixture.CreateSubscription(_topicId, subscriptionId);
    }

    private async Task PublishFakeMessage(PublisherClient publisher, PubSubMessage<CustomerProfileUpdatedMessage> customerProfileUpdateMessage)
    {
        await publisher.PublishAsync(JsonConvert.SerializeObject(customerProfileUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldIncrementReceivedMessageCount_WhenSubscriberReceivesAnIncomingMessage()
    {
        // Arrange
        var customerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>();
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var tags = new List<string>
        {
            MetricTags.StatusSuccess
        };

        // Act
        await PublishFakeMessage(publisher, customerProfileUpdateMessage);

        var result = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.CustomerProfileUpdateSubscriberReceivedMessage),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldIncrementHandleMessageCount_WhenThereIsNoRelevantInformationInIncomingMessage()
    {
        // Arrange
        var customerProfileUpdateMessage =
            CustomerProfileUpdateMessageFaker.GenerateCustomerProfileUpdateMessageWithNoRelevantInformation();
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var tags = new List<string>
        {
            MetricTags.StatusSuccess,
            CustomerProfileUpdateMetricTags.ActionIgnoreMessage
        };

        // Act
        await PublishFakeMessage(publisher, customerProfileUpdateMessage);

        var result = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.CustomerProfileUpdateSubscriberHandleMessage),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldIncrementHandleMessageCount_WhenHasUpdatedPhoneNumbersInIncomingMessage()
    {
        // Arrange
        var customerProfileUpdateMessage =
            CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessage();
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var tags = new List<string>
        {
            MetricTags.StatusSuccess,
            CustomerProfileUpdateMetricTags.ActionIdentityIntelligence,
            CustomerProfileUpdateMetricTags.ActionSimIntelligence
        };

        _customerRepository
            .GetProfileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())
            .ReturnsForAnyArgs(Task.FromResult(PersonFaker.GenerateValidPersonResponse()));

        // Act
        await PublishFakeMessage(publisher, customerProfileUpdateMessage);

        var result = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.CustomerProfileUpdateSubscriberHandleMessage),
                Arg.Is<List<string>>(l => l.All(tags.Contains) && l.Count == tags.Count)
            )
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldIncrementHandleMessageCount_WhenHasUpdatedEmailInIncomingMessage()
    {
        // Arrange
        var customerProfileUpdateMessage =
            CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithEmail();
        customerProfileUpdateMessage.Data!.PhoneNumbers = null;

        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var tags = new List<string>
        {
            MetricTags.StatusSuccess,
            CustomerProfileUpdateMetricTags.ActionEmailIntelligence
        };

        _customerRepository
            .GetProfileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())
            .ReturnsForAnyArgs(Task.FromResult(PersonFaker.GenerateValidPersonResponse()));

        // Act
        await PublishFakeMessage(publisher, customerProfileUpdateMessage);

        var result = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.CustomerProfileUpdateSubscriberHandleMessage),
                Arg.Is<List<string>>(l => l.All(tags.Contains) && l.Count == tags.Count)
            )
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldIncrementHandleMessageCount_WhenHasOnHoldDomesticAddressInIncomingMessage()
    {
        // Arrange
        var customerProfileUpdateMessage =
            CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithValidDomesticAddress();
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var tags = new List<string>
        {
            MetricTags.StatusSuccess,
            CustomerProfileUpdateMetricTags.ActionIdentityIntelligence,
            CustomerProfileUpdateMetricTags.ActionSimIntelligence,
            CustomerProfileUpdateMetricTags.ActionEmailIntelligence
        };

        _customerRepository
            .GetProfileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())
            .ReturnsForAnyArgs(Task.FromResult(PersonFaker.GenerateValidPersonResponse()));

        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        await PublishFakeMessage(publisher, customerProfileUpdateMessage);

        var result = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.CustomerProfileUpdateSubscriberHandleMessage),
                Arg.Is<List<string>>(l => l.All(tags.Contains) && l.Count == tags.Count)
            )
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldIncrementHandleMessageCount_WhenHasNoOnHoldDomesticAddressInIncomingMessage()
    {
        // Arrange
        var customerProfileUpdateMessage =
            CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithValidDomesticAddress();
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var tags = new List<string>
        {
            MetricTags.StatusSuccess,
            CustomerProfileUpdateMetricTags.ActionOnHold
        };

        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Throws(new CrmException("Test Exception"));

        // Act
        await PublishFakeMessage(publisher, customerProfileUpdateMessage);

        var result = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.CustomerProfileUpdateSubscriberHandleMessage),
                Arg.Is<List<string>>(l => l.All(tags.Contains) && l.Count == tags.Count)
            )
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldIncrementHandleMessageCount_WhenFailToHandleMessage()
    {
        // Arrange
        var customerProfileUpdateMessage =
            CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessage();
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var tags = new List<string>
        {
            MetricTags.StatusPermanentError
        };

        _customerRepository
            .GetProfileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())
            .Throws(new Exception("Test Exception"));

        // Act
        await PublishFakeMessage(publisher, customerProfileUpdateMessage);

        var result = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.CustomerProfileUpdateSubscriberHandleMessage),
                Arg.Is<List<string>>(l => l.All(tags.Contains) && l.Count == tags.Count)
            )
        );

        // Assert
        Assert.Null(result);
    }
}
