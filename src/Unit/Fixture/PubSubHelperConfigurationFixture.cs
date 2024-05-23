using Bogus;
using Infra.Config.PubSub;

namespace Unit.Fixture;

public class PubSubHelperConfigurationFixture
{
    private readonly Faker<EmailIntelligencePublisherConfiguration> _fakerPublisherConfigurationEmailIntelligence = new();

    private readonly Faker<IdentityIntelligencePublisherConfiguration> _fakerPublisherConfigurationIdentityIntelligence = new();

    private readonly Faker<SimIntelligencePublisherConfiguration> _fakerPublisherConfigurationSimIntelligence = new();

    private readonly Faker<CustomerProfileUpdateSubscriberConfiguration> _fakerSubscriberConfiguration = new();

    private readonly Faker<CustomerProfileUpdateSubscriberConfiguration> _fakerSubscriberPiiConfiguration = new();

    public CustomerProfileUpdateSubscriberConfiguration GenerateValidPiiSubscriberConfiguration()
    {
        return _fakerSubscriberPiiConfiguration
            .RuleFor(configuration => configuration.Enable, true)
            .RuleFor(configuration => configuration.ProjectId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.SubscriptionId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.UseEmulator, false);
    }

    public CustomerProfileUpdateSubscriberConfiguration GenerateValidSubscriberConfiguration()
    {
        return _fakerSubscriberConfiguration
            .RuleFor(configuration => configuration.Enable, true)
            .RuleFor(configuration => configuration.ProjectId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.SubscriptionId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.UseEmulator, false);
    }

    public SimIntelligencePublisherConfiguration GenerateValidPublisherConfigurationForSimIntelligence()
    {
        return _fakerPublisherConfigurationSimIntelligence
            .RuleFor(configuration => configuration.Enable, true)
            .RuleFor(configuration => configuration.ProjectId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.TopicId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.UseEmulator, false);
    }

    public EmailIntelligencePublisherConfiguration GenerateValidPublisherConfigurationForEmailIntelligence()
    {
        return _fakerPublisherConfigurationEmailIntelligence
            .RuleFor(configuration => configuration.Enable, true)
            .RuleFor(configuration => configuration.ProjectId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.TopicId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.UseEmulator, false);
    }

    public IdentityIntelligencePublisherConfiguration GenerateValidPublisherConfigurationForIdentityIntelligence()
    {
        return _fakerPublisherConfigurationIdentityIntelligence
            .RuleFor(configuration => configuration.Enable, true)
            .RuleFor(configuration => configuration.ProjectId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.TopicId, faker => faker.Random.Word())
            .RuleFor(configuration => configuration.UseEmulator, false);
    }
}
