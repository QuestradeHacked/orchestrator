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
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Questrade.Library.PubSubClientHelper.Primitives;
using System.Reflection;
using Xunit;

namespace Integration.Infra.Subscriber;

public class CustomerProfileUpdateSubscriberHandleReceivedMessageAsyncTests : IAssemblyFixture<PubSubEmulatorFixture>
{
    private readonly IMediator _mediator;

    private readonly CustomerProfileUpdateSubscriber _subscriber;

    private readonly ICustomerOnHoldRepository _customerOnHoldRepository;

    private readonly ICustomerRepository _customerRepository;

    public CustomerProfileUpdateSubscriberHandleReceivedMessageAsyncTests(PubSubEmulatorFixture pubSubEmulatorFixture)
    {
        LoggerFixture<CustomerProfileUpdateSubscriberHandleReceivedMessageAsyncTests> loggerFixture = new();
        Mock<ILoggerFactory> loggerFactory = new();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerFixture);

        _mediator = Substitute.For<IMediator>();
        var metricService = Substitute.For<IMetricService>();
        var topicId = $"T_{Guid.NewGuid()}";

        var services = new ServiceCollection();
        services.AddSingleton<CorrelationContext>();
        services.AddSingleton<IDefaultJsonSerializerOptionsProvider, MyDefaultJsonSerializerOptionsProvider>();
        var serviceProvider = services.AddMemoryCache().BuildServiceProvider();

        var subscriptionId = $"{topicId}.{Guid.NewGuid()}";
        var subscriberConfig = pubSubEmulatorFixture.CreateDefaultSubscriberConfig(subscriptionId);

        _customerOnHoldRepository = Substitute.For<ICustomerOnHoldRepository>();
        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        _customerOnHoldRepository.UpsertAsync(Arg.Any<CustomerOnHold>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(string.Empty));
        _customerOnHoldRepository.ReleaseCustomerOnHoldAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));


        _customerRepository = Substitute.For<ICustomerRepository>();
        _customerRepository
            .GetProfileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(PersonFaker.GenerateValidPersonResponse());

        _subscriber = new CustomerProfileUpdateSubscriber(
            loggerFactory.Object,
            subscriberConfig,
            serviceProvider,
            _mediator,
            metricService,
            _customerRepository,
            _customerOnHoldRepository
        );

        pubSubEmulatorFixture.CreateTopic(topicId);
        pubSubEmulatorFixture.CreateSubscription(topicId, subscriptionId);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenPersonalNumberIsSent()
    {
        // Arrange
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessage();
        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.Received().Send(Arg.Any<CustomerProfilePhoneUpdatedRequest>());
        Assert.True(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenWorkPhoneNumberIsSent()
    {
        // Arrange
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateInvalidWorkPhoneCustomerProfileUpdateMessage();

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.DidNotReceive().Send(Arg.Any<CustomerProfilePhoneUpdatedRequest>());
        Assert.True(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenPersonalAndWorkPhoneNumberIsSent()
    {
        // Arrange
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateCustomerProfileUpdateMessageWithPersonalAndWorkPhoneNumbers();
        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.Received().Send(Arg.Any<CustomerProfilePhoneUpdatedRequest>());
        Assert.True(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenEmailIsSent()
    {
        // Arrange
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithEmail();
        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.Received().Send(Arg.Any<CustomerProfileEmailUpdatedRequest>());
        Assert.True(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenEmailAndPhoneNumberIsSent()
    {
        // Arrange
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithEmailAndPhoneNumber();
        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.ReceivedWithAnyArgs(3).Send(Arg.Any<IRequest>());
        Assert.True(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnFalse_WhenMediatorSendThrowError()
    {
        // Arrange
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessage();
        _mediator
            .Send(Arg.Any<CustomerProfilePhoneUpdatedRequest>(), Arg.Any<CancellationToken>())
            .Throws(new Exception());
        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        _customerOnHoldRepository.ReleaseCustomerOnHoldAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.Received().Send(Arg.Any<CustomerProfilePhoneUpdatedRequest>());
        Assert.False(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenUserIsNotFound()
    {
        // Arrange
        _customerRepository
            .GetProfileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())
            .ReturnsForAnyArgs(Task.FromResult(Tuple.Create<CrmPerson?, PersonAccount?>(null, null)));
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessage();

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        Assert.True(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenUserHasNoAddress()
    {
        // Arrange
        _customerRepository
            .GetProfileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())
            .ReturnsForAnyArgs(Task.FromResult(PersonFaker.GenerateValidPersonResponseWithNoAddress()));

        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithEmail();

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        Assert.True(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnFalse_WhenUserHasInternationalAddress()
    {
        // Arrange
        _customerRepository
            .GetProfileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())
            .ReturnsForAnyArgs(Task.FromResult(PersonFaker.GenerateValidPersonResponseWithInternationalAddress()));

        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));
        _customerOnHoldRepository.ReleaseCustomerOnHoldAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithEmail();

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.ReceivedWithAnyArgs(3).Send(Arg.Any<IRequest>());
        Assert.True(success);

    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenUserHasValidDomesticAddress()
    {
        // Arrange
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithValidDomesticAddress();

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.Received().Send(Arg.Any<CustomerProfileEmailUpdatedRequest>());
        Assert.True(success);
    }

    [Fact]
    public async Task HandleReceivedMessageAsync_ShouldReturnTrue_WhenUserHasValidDomesticAddressAndNotIsOnHold()
    {
        // Arrange
        var customerPubSubMessage = CustomerProfileUpdateMessageFaker.GenerateValidCustomerProfileUpdateMessageWithValidDomesticAddress();
        _customerOnHoldRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var success = await HandleReceivedMessageAsync(customerPubSubMessage, CancellationToken.None)!;

        // Assert
        await _mediator.DidNotReceive().Send(Arg.Any<CustomerProfilePhoneUpdatedRequest>());
        Assert.True(success);
    }

    private Task<bool>? HandleReceivedMessageAsync(PubSubMessage<CustomerProfileUpdatedMessage> message,
        CancellationToken cancellationToken)
    {
        var typeSubscription = _subscriber.GetType();

        var handleMethod = typeSubscription.GetMethod("HandleReceivedMessageAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        var response = handleMethod!.Invoke(_subscriber, new object[] { message, cancellationToken });

        return response as Task<bool>;
    }
}
