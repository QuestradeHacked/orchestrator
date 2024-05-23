using Infra.Config.PubSub;

namespace Orchestrator.Config;

public class OrchestratorConfiguration
{
    public CustomerProfileUpdateSubscriberConfiguration? CustomerProfileUpdateSubscriberConfiguration { get; set; }

    public CustomerPiiUpdateSubscriberConfiguration? CustomerPiiUpdateSubscriberConfiguration { get; set; }

    public SimIntelligencePublisherConfiguration? SimIntelligencePublisherConfiguration { get; set; }

    public IdentityIntelligencePublisherConfiguration? IdentityIntelligencePublisherConfiguration { get; set; }

    public EmailIntelligencePublisherConfiguration? EmailIntelligencePublisherConfiguration { get; set; }

    internal void Validate()
    {
        if (CustomerProfileUpdateSubscriberConfiguration is null)
        {
            throw new InvalidOperationException("The customer profile updated subscriber configuration is not valid.");
        }

        CustomerProfileUpdateSubscriberConfiguration.Validate();

        if (CustomerPiiUpdateSubscriberConfiguration is null)
        {
            throw new InvalidOperationException("The customer pii subscriber configuration is not valid.");
        }

        CustomerPiiUpdateSubscriberConfiguration.Validate();

        if (SimIntelligencePublisherConfiguration is null)
        {
            throw new InvalidOperationException("The sim intelligence publisher configuration is not valid.");
        }

        SimIntelligencePublisherConfiguration.Validate();

        if (IdentityIntelligencePublisherConfiguration is null)
        {
            throw new InvalidOperationException("The identity intelligence publisher configuration is not valid.");
        }

        IdentityIntelligencePublisherConfiguration.Validate();

        if (EmailIntelligencePublisherConfiguration is null)
        {
            throw new InvalidOperationException("The email intelligence publisher configuration is not valid.");
        }

        EmailIntelligencePublisherConfiguration.Validate();
    }
}
