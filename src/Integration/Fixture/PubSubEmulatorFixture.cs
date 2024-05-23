using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Infra.Config.PubSub;
using Integration.Config;

namespace  Integration.Fixture;

public class PubSubEmulatorFixture
{
    private readonly AppSettings _appSettings = new();

    private readonly PublisherServiceApiClient _publisherServiceApiClient;

    private readonly SubscriberServiceApiClient _subscriberServiceApiClient;

    private string Endpoint { get; }

    private string ProjectId { get; }

    public int SubscriberTimeout { get; }

    public PubSubEmulatorFixture()
    {
        Endpoint = $"{_appSettings.GetProcessPubSubHost()}:{_appSettings.GetProcessPubSubPort()}";
        Environment.SetEnvironmentVariable("PUBSUB_EMULATOR_HOST", Endpoint);

        ProjectId = _appSettings.GetPubSubProjectId();
        SubscriberTimeout = _appSettings.GetPubSubSubscriberTimeout();

        _publisherServiceApiClient = CreatePublisherServiceApiClient();
        _subscriberServiceApiClient = CreateSubscriberServiceApiClient();
    }

    public CustomerProfileUpdateSubscriberConfiguration CreateDefaultSubscriberConfig(string subscriptionId)
    {
        return new CustomerProfileUpdateSubscriberConfiguration
        {
            AcknowledgeDeadline = TimeSpan.FromSeconds(30),
            AcknowledgeExtensionWindow = TimeSpan.FromSeconds(10),
            Enable = true,
            Endpoint = Endpoint,
            MaximumOutstandingByteCount = 1,
            MaximumOutstandingElementCount = 1,
            ProjectId = ProjectId,
            SubscriberClientCount = 1,
            SubscriptionId = subscriptionId,
            UseEmulator = true
        };
    }

    public CustomerPiiUpdateSubscriberConfiguration CreateDefaultSubscriberPiiConfig(string subscriptionId)
    {
        return new CustomerPiiUpdateSubscriberConfiguration
        {
            AcknowledgeDeadline = TimeSpan.FromSeconds(30),
            AcknowledgeExtensionWindow = TimeSpan.FromSeconds(10),
            Enable = true,
            Endpoint = Endpoint,
            MaximumOutstandingByteCount = 1,
            MaximumOutstandingElementCount = 1,
            ProjectId = ProjectId,
            SubscriberClientCount = 1,
            SubscriptionId = subscriptionId,
            UseEmulator = true
        };
    }

    public async Task<PublisherClient> CreatePublisherAsync(string topicId)
    {
        var publisherClientBuilder = new PublisherClientBuilder
        {
            ApiSettings = PublisherServiceApiSettings.GetDefault(),
            ChannelCredentials = ChannelCredentials.Insecure,
            Endpoint = Endpoint,
            Settings = new PublisherClient.Settings{
                EnableMessageOrdering = true
            },
            TopicName = TopicName.FromProjectTopic(ProjectId, topicId)
        };

        var publisher = await publisherClientBuilder.BuildAsync();

        return publisher;
    }

    private PublisherServiceApiClient CreatePublisherServiceApiClient()
    {
        var publisher = new PublisherServiceApiClientBuilder
        {
            ChannelCredentials = ChannelCredentials.Insecure,
            Endpoint = Endpoint
        }.Build();

        return publisher;
    }

    private SubscriberServiceApiClient CreateSubscriberServiceApiClient()
    {
        var subscriber = new SubscriberServiceApiClientBuilder
        {
            ChannelCredentials = ChannelCredentials.Insecure,
            Endpoint = Endpoint
        }.Build();

        return subscriber;
    }

    public Subscription CreateSubscription(string topicId, string subscriptionId)
    {
        var topicName = TopicName.FromProjectTopic(ProjectId, topicId);
        var subscriptionName = SubscriptionName.FromProjectSubscription(ProjectId, subscriptionId);

        var subscription = _subscriberServiceApiClient.CreateSubscription(
            new Subscription{
                TopicAsTopicName = topicName,
                SubscriptionName = subscriptionName,
                EnableMessageOrdering = true
            }
        );

        Console.WriteLine($"Subscription {subscriptionId} created.");

        return subscription;
    }

    public Topic CreateTopic(string topicId)
    {
        var topicName = TopicName.FromProjectTopic(ProjectId, topicId);
        var topic = _publisherServiceApiClient.CreateTopic(topicName);

        Console.WriteLine($"Topic {topic.Name} created.");

        return topic;
    }
}
