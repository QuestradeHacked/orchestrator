using Domain.Models;
using Domain.Models.Pii;
using Microsoft.Extensions.Logging;
using Questrade.Library.PubSubClientHelper.Primitives;

namespace Infra.Config.PubSub;

public class CustomerPiiUpdateSubscriberConfiguration : BaseSubscriberConfiguration<PubSubMessage<CustomerPiiUpdateMessage>>
{
    public override Task HandleMessageLogAsync(
        ILogger logger,
        LogLevel logLevel,
        PubSubMessage<CustomerPiiUpdateMessage> message,
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
            throw new InvalidOperationException("The configuration options for the customer Pii subscriber is not valid");
        }

        if (UseEmulator && string.IsNullOrWhiteSpace(Endpoint))
        {
            throw new InvalidOperationException("The emulator configuration options for customer Pii subscriber is not valid");
        }
    }
}
