using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Pii;
using Domain.Repositories.Firestore;
using Domain.Repositories.GraphQL;
using Domain.Services;
using Infra.Config.PubSub;
using Infra.Constants;
using Infra.Subscriber.Extensions;
using Infra.Util;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Questrade.Library.PubSubClientHelper.Subscriber;

namespace Infra.Subscriber;

public class CustomerProfileUpdateSubscriber : PubsubSubscriberBackgroundService<
    CustomerProfileUpdateSubscriberConfiguration, PubSubMessage<CustomerProfileUpdatedMessage>>
{
    private readonly ICustomerOnHoldRepository _customerOnHoldRepository;

    private readonly ICustomerRepository _customerRepository;

    private readonly IMediator _mediator;

    private readonly IMetricService _metricService;

    private readonly IServiceProvider _serviceProvider;

    public CustomerProfileUpdateSubscriber(ILoggerFactory logger,
        CustomerProfileUpdateSubscriberConfiguration subscriberConfiguration,
        IServiceProvider serviceProvider,
        IMediator mediator,
        IMetricService metricService,
        ICustomerRepository customerRepository,
        ICustomerOnHoldRepository customerOnHoldRepository)
        : base(logger, subscriberConfiguration, serviceProvider)
    {
        _customerOnHoldRepository = customerOnHoldRepository;
        _customerRepository = customerRepository;
        _mediator = mediator;
        _metricService = metricService;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<bool> HandleReceivedMessageAsync(PubSubMessage<CustomerProfileUpdatedMessage> message,
        CancellationToken cancellationToken)
    {
        _logDefineScope(Logger, nameof(CustomerProfileUpdateSubscriber), nameof(HandleReceivedMessageAsync));
        using var scope = _serviceProvider.CreateScope();

        var correlationContext = scope.ServiceProvider.GetRequiredService<CorrelationContext>();
        var changeRequests = new List<IRequestOrchestrator>();
        var metricTags = new List<string>();

        correlationContext.AddCorrelationIdScope(Logger, Guid.NewGuid().ToString());

        _logMessageReceivedInformation(Logger, message.Id, null);
        _metricService.Increment(MetricNames.CustomerProfileUpdateSubscriberReceivedMessage,
            new List<string> { MetricTags.StatusSuccess});

        try
        {
            var data = message.Data!;
            var phoneChangeRequest = data.HandlePhoneChangesForSimIntelligence();

            if (phoneChangeRequest is not null)
            {
                var identityChangeRequest = data.HandlePhoneChangesForIdentityIntelligence()!;

                _logPhoneNumberUpdatedInformation(Logger, message.Id, null);

                changeRequests.Add(phoneChangeRequest);
                changeRequests.Add(identityChangeRequest);
                metricTags.Add(CustomerProfileUpdateMetricTags.ActionIdentityIntelligence);
                metricTags.Add(CustomerProfileUpdateMetricTags.ActionSimIntelligence);
            }

            var emailChangeRequest = data.HandleEmailChanges();

            if (emailChangeRequest is not null)
            {
                _logEmailUpdatedInformation(Logger, message.Id, null);

                changeRequests.Add(emailChangeRequest);
                metricTags.Add(CustomerProfileUpdateMetricTags.ActionEmailIntelligence);
            }

            var isCustomerOnHold =
                await _customerOnHoldRepository.ExistsAsync(data.CustomerFirestoreId(), cancellationToken);

            if (isCustomerOnHold)
            {
                changeRequests.Clear();
                metricTags.Clear();

                if (data.HasDomesticAddressChanges())
                {
                    _logDomesticAddressUpdatedInformation(Logger, message.Id, null);

                    changeRequests.AddRange(data.HandleDomesticAddressChanges());
                    await _customerOnHoldRepository.ReleaseCustomerOnHoldAsync(data.CustomerFirestoreId(),
                        cancellationToken);

                    metricTags.Add(CustomerProfileUpdateMetricTags.ActionEmailIntelligence);
                    metricTags.Add(CustomerProfileUpdateMetricTags.ActionIdentityIntelligence);
                    metricTags.Add(CustomerProfileUpdateMetricTags.ActionSimIntelligence);
                }
            }

            if (!changeRequests.Any())
            {
                _logMissingRelevantInformationWarning(Logger, message.Id, null);
                _metricService.Increment(MetricNames.CustomerProfileUpdateSubscriberHandleMessage,
                    new List<string>
                    {
                        MetricTags.StatusSuccess,
                        CustomerProfileUpdateMetricTags.ActionIgnoreMessage
                    }
                );

                return true;
            }

            var (person, personAccount) =
                await _customerRepository.GetProfileAsync(data.UserId!, data.PersonId!, cancellationToken);

            if (person is null)
            {
                throw new PersonNotFoundException();
            }

            foreach (var request in changeRequests)
            {
                request.AddCustomerInfo(person, personAccount);
            }

            await Task.WhenAll(changeRequests.Select(cr => _mediator.Send(cr, cancellationToken)));

            metricTags.Add(MetricTags.StatusSuccess);
            _metricService.Increment(MetricNames.CustomerProfileUpdateSubscriberHandleMessage,
                metricTags);

            return true;
        }
        catch (BusinessException businessEx)
        {
            _logBusinessExceptionWarning(Logger, businessEx);
            _metricService.Increment(MetricNames.CustomerProfileUpdateSubscriberHandleMessage,
                new List<string>
                {
                    MetricTags.StatusSuccess,
                    CustomerProfileUpdateMetricTags.ActionOnHold
                }
            );

            return true;
        }
        catch (Exception error)
        {
            _logHandlingMessageError(Logger, nameof(CustomerProfileUpdateSubscriber), message.Id, error);
            _metricService.Increment(MetricNames.CustomerProfileUpdateSubscriberHandleMessage,
                new List<string> { MetricTags.StatusPermanentError });

            return false;
        }
    }

    private readonly Func<ILogger, string?, string?, IDisposable> _logDefineScope =
        LoggerMessage.DefineScope<string?, string?>(
            formatString: "{CustomerProfileUpdateSubscriber}:{HandleReceivedMessageAsyncName}"
        );

    private readonly Action<ILogger, string?, Exception?> _logMessageReceivedInformation =
        LoggerMessage.Define<string?>(
            eventId: new EventId(1, nameof(CustomerProfileUpdateSubscriber)),
            formatString: "Message received: {MessageId}",
            logLevel: LogLevel.Information
        );

    private readonly Action<ILogger, string?, Exception?> _logMissingRelevantInformationWarning =
        LoggerMessage.Define<string?>(
            eventId: new EventId(2, nameof(CustomerProfileUpdateSubscriber)),
            formatString: "The message {MessageId} doesn't contains relevant updates for Orchestrator",
            logLevel: LogLevel.Warning
        );

    private readonly Action<ILogger, string?, Exception?> _logPhoneNumberUpdatedInformation =
        LoggerMessage.Define<string?>(
            eventId: new EventId(3, nameof(CustomerProfileUpdateSubscriber)),
            formatString: "The message {MessageId} has a phone number updated",
            logLevel: LogLevel.Information
        );

    private readonly Action<ILogger, string?, Exception?> _logEmailUpdatedInformation =
        LoggerMessage.Define<string?>(
            eventId: new EventId(4, nameof(CustomerProfileUpdateSubscriber)),
            formatString: "The message {MessageId} has an e-mail updated",
            logLevel: LogLevel.Information
        );

    private readonly Action<ILogger, Exception?> _logBusinessExceptionWarning =
        LoggerMessage.Define(
            eventId: new EventId(5, nameof(CustomerProfileUpdateSubscriber)),
            formatString: "Customer onboarding not completed.",
            logLevel: LogLevel.Warning
        );

    private readonly Action<ILogger, string?, string?, Exception?> _logHandlingMessageError =
        LoggerMessage.Define<string?, string?>(
            eventId: new EventId(6, nameof(CustomerProfileUpdateSubscriber)),
            formatString: "Failed on handling the received message from {CustomerProfileUpdateSubscriber}: {MessageId}",
            logLevel: LogLevel.Error
        );

    private readonly Action<ILogger, string?, Exception?> _logDomesticAddressUpdatedInformation =
        LoggerMessage.Define<string?>(
            eventId: new EventId(7, nameof(CustomerProfileUpdateSubscriber)),
            formatString: "The message {MessageId} has a valid domestic address updated",
            logLevel: LogLevel.Information
        );
}
