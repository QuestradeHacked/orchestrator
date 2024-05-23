using Domain.Models;
using Microsoft.Extensions.Logging;
using Questrade.Library.PubSubClientHelper.Primitives;

namespace Infra.Config.PubSub;

public class CustomerProfileUpdateSubscriberConfiguration : BaseSubscriberConfiguration<PubSubMessage<CustomerProfileUpdatedMessage>>
{
    public override Task HandleMessageLogAsync(
        ILogger logger,
        LogLevel logLevel,
        PubSubMessage<CustomerProfileUpdatedMessage> message,
        string logMessage,
        Exception error = null!,
        CancellationToken cancellationToken = default
    )
    {
        logger.Log(logLevel, "{@LogMessage} - Message contents: {@Message}", logMessage, message);

        return Task.CompletedTask;
    }

    public void Validate()
    {
        if (!Enable) return;

        if (string.IsNullOrWhiteSpace(ProjectId) || string.IsNullOrWhiteSpace(SubscriptionId))
        {
            throw new InvalidOperationException("The configuration options for the customer profile updated subscriber is not valid");
        }

        if (UseEmulator && string.IsNullOrWhiteSpace(Endpoint))
        {
            throw new InvalidOperationException("The emulator configuration options for customer profile updated subscriber is not valid");
        }
    }
}
