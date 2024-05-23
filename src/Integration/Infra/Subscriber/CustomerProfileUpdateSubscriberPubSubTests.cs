using Domain.Entities;
using Domain.Models;
using Domain.Models.CRM;
using Domain.Repositories.Firestore;
using Domain.Repositories.GraphQL;
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
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questrade.Library.PubSubClientHelper.Primitives;
using Xunit;

namespace Integration.Infra.Subscriber;

public class CustomerProfileUpdateSubscriberPubSubTests : IAssemblyFixture<PubSubEmulatorFixture>
{
    private readonly LoggerFixture<CustomerProfileUpdateSubscriberPubSubTests> _loggerFixture;

    private readonly IMediator _mediator;

    private readonly PubSubEmulatorFixture _pubSubFixture;

    private readonly CustomerProfileUpdateSubscriber _subscriber;

    private readonly int _subscriberTimeout;

    private readonly string _topicId;

    public CustomerProfileUpdateSubscriberPubSubTests(PubSubEmulatorFixture pubSubEmulatorFixture)
    {
        _loggerFixture = new LoggerFixture<CustomerProfileUpdateSubscriberPubSubTests>();
        Mock<ILoggerFactory> loggerFactory = new();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_loggerFixture);

         _mediator = Substitute.For<IMediator>();
        var metricService = Substitute.For<IMetricService>();
        _pubSubFixture = pubSubEmulatorFixture;
        _subscriberTimeout = _pubSubFixture.SubscriberTimeout;
        _topicId = $"T_{Guid.NewGuid()}";


        var services = new ServiceCollection();
        services.AddSingleton<CorrelationContext>();
        services.AddSingleton<IDefaultJsonSerializerOptionsProvider, MyDefaultJsonSerializerOptionsProvider>();
        var serviceProvider = services.AddMemoryCache().BuildServiceProvider();

        var subscriptionId = $"{_topicId}.{Guid.NewGuid()}";
        var subscriberConfig = _pubSubFixture.CreateDefaultSubscriberConfig(subscriptionId);

        var  customerOnHoldRepository = Substitute.For<ICustomerOnHoldRepository>();
        customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        customerOnHoldRepository.UpsertAsync(Arg.Any<CustomerOnHold>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(string.Empty));

        var customerRepository = Substitute.For<ICustomerRepository>();
        customerRepository
            .GetProfileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Tuple.Create<CrmPerson?,PersonAccount?>(new CrmPerson(), new PersonAccount()));

        _subscriber = new CustomerProfileUpdateSubscriber(
            loggerFactory.Object,
            subscriberConfig,
            serviceProvider,
            _mediator,
            metricService,
            customerRepository,
            customerOnHoldRepository
        );

        _pubSubFixture.CreateTopic(_topicId);
        _pubSubFixture.CreateSubscription(_topicId, subscriptionId);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldLogInformation_WhenSubscriberReceivesAnIncomingMessage()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>();

        // Act
        await publisher.PublishAsync(JsonConvert.SerializeObject(customerProfileUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains("Message received", loggedMessages);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldLogError_WhenFailToHandleMessage()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerProfileUpdateMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessage();

        _mediator.Send(new CustomerProfilePhoneUpdatedRequest(), CancellationToken.None).ThrowsForAnyArgs(new Exception());

        // Act
        await publisher.PublishAsync(JsonConvert.SerializeObject(customerProfileUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains("Failed on handling the received message from", loggedMessages);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldLogInformation_WhenHasUpdatedPhoneNumbersInIncomingMessage()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerProfileUpdateMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessage();

        // Act
        await publisher.PublishAsync(JsonConvert.SerializeObject(customerProfileUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains("Message received", loggedMessages);
        Assert.Contains($"The message {customerProfileUpdateMessage.Id} has a phone number updated", loggedMessages);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldLogInformation_WhenThereIsNoRelevantInformationInIncomingMessage()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var faker = new Bogus.Faker();
        var customerProfileUpdateMessage = new PubSubMessage<CustomerProfileUpdatedMessage>
        {
            Id = faker.Random.Uuid().ToString(),
            Data = new CustomerProfileUpdatedMessage
            {
                UserId = faker.Random.Int(0).ToString()
            }
        };

        // Act
        await publisher.PublishAsync(JsonConvert.SerializeObject(customerProfileUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains("Message received", loggedMessages);
        Assert.Contains($"The message {customerProfileUpdateMessage.Id} doesn't contains relevant updates for Orchestrator", loggedMessages);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldLogInformation_WhenHasUpdatedWorkPhoneNumbersInIncomingMessage()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerProfileUpdateMessage = CustomerProfileUpdateMessageFaker.GenerateInvalidWorkPhoneCustomerProfileUpdateMessage();

        // Act
        await publisher.PublishAsync(JsonConvert.SerializeObject(customerProfileUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains("Message received", loggedMessages);
        Assert.Contains($"The message {customerProfileUpdateMessage.Id} doesn't contains relevant updates for Orchestrator", loggedMessages);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldLogInformation_WhenHasUpdatedPersonalAndWorkPhoneNumbersInIncomingMessage()
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);
        var customerProfileUpdateMessage = CustomerProfileUpdateMessageFaker.GenerateCustomerProfileUpdateMessageWithPersonalAndWorkPhoneNumbers();


        // Act
        await publisher.PublishAsync(JsonConvert.SerializeObject(customerProfileUpdateMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _loggerFixture.GetAllMessages();

        // Assert
        Assert.Contains("Message received", loggedMessages);
        Assert.Contains($"The message {customerProfileUpdateMessage.Id} has a phone number updated", loggedMessages);
    }
}
