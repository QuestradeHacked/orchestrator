using Domain.Models;
using Microsoft.Extensions.Logging;
using Questrade.Library.PubSubClientHelper.Primitives;

namespace Infra.Config.PubSub;

public class SimIntelligencePublisherConfiguration : BasePublisherConfiguration<SimIntelligencePublisherMessage>
{
    public override Task HandleMessageLogAsync(ILogger logger, LogLevel logLevel, SimIntelligencePublisherMessage message, string logMessage, CancellationToken cancellationToken = default)
    {
        logger.Log(logLevel, "{LogMessage} - Message contents: {@Message}", logMessage, message);

        return Task.CompletedTask;
    }

    public virtual void Validate()
    {
        if (!Enable)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ProjectId))
        {
            throw new InvalidOperationException($"The configuration options for the {ProjectId} publisher are not valid");
        }

        if (string.IsNullOrWhiteSpace(TopicId))
        {
            throw new InvalidOperationException($"The configuration options for the {TopicId} publisher are not valid");
        }

        if (UseEmulator && string.IsNullOrWhiteSpace(Endpoint))
        {
            throw new InvalidOperationException($"The emulator configuration options for {Endpoint} publisher are not valid");
        }
    }
}
