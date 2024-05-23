using Domain.Constants;
using Domain.Models;
using Domain.Models.Analysis;
using Domain.Models.Pii;
using Domain.Services;
using Infra.Config.PubSub;
using Infra.Subscriber.Extensions.Pii;
using Infra.Util;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Questrade.Library.PubSubClientHelper.Subscriber;

namespace Infra.Subscriber;

public class CustomerPiiUpdateSubscriber : PubsubSubscriberBackgroundService<
    CustomerPiiUpdateSubscriberConfiguration, PubSubMessage<CustomerPiiUpdateMessage>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMetricService _metricService;
    private readonly IMediator _mediator;

    public CustomerPiiUpdateSubscriber
    (
        ILoggerFactory loggerFactory,
        CustomerPiiUpdateSubscriberConfiguration subscriberConfiguration,
        IServiceProvider serviceProvider,
        IMetricService metricService, IMediator mediator)
        : base(loggerFactory, subscriberConfiguration, serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _metricService = metricService;
        _mediator = mediator;
    }


    protected override async Task<bool> HandleReceivedMessageAsync(PubSubMessage<CustomerPiiUpdateMessage> message, CancellationToken cancellationToken)
    {
        _logDefineScope(Logger, nameof(CustomerPiiUpdateMessage), nameof(HandleReceivedMessageAsync));
        using var scope = _serviceProvider.CreateScope();

        var correlationContext = scope.ServiceProvider.GetRequiredService<CorrelationContext>();
        var metricTags = new List<string>();

        correlationContext.AddCorrelationIdScope(Logger, Guid.NewGuid().ToString());

        _logMessageReceivedInformation(Logger, message.Id, null);

        _metricService.Increment(MetricNames.CustomerPiiUpdateSubscriberReceivedMessage,
            new List<string> { MetricTags.StatusSuccess });

        try
        {
            var analysisRequests = new List<AnalysisRequest>();
            var customerPiiUpdateMessage = message.Data!;

            if
            (
                customerPiiUpdateMessage.HasPrimaryAddressCreated() ||
                (
                    customerPiiUpdateMessage.HasPrimaryAddress() &&
                    customerPiiUpdateMessage.HasPrimaryEmailChanges()
                )
            )
            {
                var emailRequest = customerPiiUpdateMessage.EmailUpdateHandler();

                if(emailRequest is not null)
                {
                    analysisRequests.Add(emailRequest);

                    _logEmailUpdatedInformation(Logger, message.Id, null);
                }
            }

            if(
                customerPiiUpdateMessage.HasPrimaryAddressCreated() ||
                (
                    customerPiiUpdateMessage.HasPrimaryAddress() &&
                    customerPiiUpdateMessage.HasPersonalPhoneChanges()
                )
            )
            {
               var phoneRequest = customerPiiUpdateMessage.PhoneUpdateHandler();

                if(phoneRequest is not null)
                {
                    analysisRequests.AddRange(phoneRequest) ;

                    _logPhoneNumberUpdatedInformation(Logger, message.Id, null);
                }
            }

            if(!analysisRequests.Any())
            {
                _logMissingRelevantInformationWarning(Logger, message.Id, null);
            }

            await Task.WhenAll(analysisRequests.Select(c => _mediator.Send(c, cancellationToken)));

            metricTags.Add(MetricTags.StatusSuccess);
            _metricService.Increment(MetricNames.CustomerPiiUpdateSubscriberHandleMessage,
                metricTags);

            return true;
        }
        catch (Exception error)
        {
            _logHandlingMessageError(Logger, nameof(CustomerPiiUpdateSubscriber), message.Id, error);
            _metricService.Increment(MetricNames.CustomerPiiUpdateSubscriberHandleMessage,
                new List<string> { MetricTags.StatusPermanentError });

            return false;
        }
    }

    private readonly Action<ILogger, string?, Exception?>
    _logEmailUpdatedInformation = LoggerMessage.Define<string?>(
            eventId: new EventId(4, nameof(CustomerPiiUpdateSubscriber)),
            formatString: "The message {MessageId} has an e-mail updated",
            logLevel: LogLevel.Information);

    private readonly Func<ILogger, string?, string?, IDisposable> _logDefineScope =
        LoggerMessage.DefineScope<string?, string?>(
            formatString: "{CustomerPiiUpdatedMessage}:{HandleReceivedMessageAsyncName}");

    private readonly Action<ILogger, string?, Exception?> _logMessageReceivedInformation = LoggerMessage.Define<string?>(
            eventId: new EventId(1, nameof(CustomerPiiUpdateSubscriber)),
            formatString: "Message received: {MessageId}",
            logLevel: LogLevel.Information);

    private readonly Action<ILogger, string?, Exception?> _logPhoneNumberUpdatedInformation =
        LoggerMessage.Define<string?>(
            eventId: new EventId(3, nameof(CustomerPiiUpdateSubscriber)),
            formatString: "The message {MessageId} has a phone number updated",
            logLevel: LogLevel.Information);

    private readonly Action<ILogger, string?, Exception?> _logMissingRelevantInformationWarning =
        LoggerMessage.Define<string?>(
            eventId: new EventId(2, nameof(CustomerPiiUpdateSubscriber)),
            formatString: "The message {MessageId} doesn't contains relevant updates for Orchestrator",
            logLevel: LogLevel.Warning);
    private readonly Action<ILogger, string?, string?, Exception?> _logHandlingMessageError =
        LoggerMessage.Define<string?, string?>(
             eventId: new EventId(6, nameof(CustomerPiiUpdateSubscriber)),
            formatString: "Failed on handling the received message from {CustomerPiiUpdateSubscriber}: {MessageId}",
            logLevel: LogLevel.Error
        );

    private readonly Action<ILogger, string?, Exception?> _logAddressUpdatedInformation =
        LoggerMessage.Define<string?>(
            eventId: new EventId(7, nameof(CustomerPiiUpdateSubscriber)),
            formatString: "The message {MessageId} has a valid address updated",
            logLevel: LogLevel.Information
        );
}
