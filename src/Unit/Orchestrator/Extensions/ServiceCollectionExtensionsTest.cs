using Domain.Configuration;
using FluentAssertions;
using Infra.Config;
using Infra.Config.PubSub;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Orchestrator.Config;
using Orchestrator.Extensions;
using Xunit;

namespace Unit.Orchestrator.Extensions;

public class ServiceCollectionExtensionsTest
{
    private readonly WebApplicationBuilder _builder;
    private readonly OrchestratorConfiguration _orchestratorConfiguration;
    private readonly Bogus.Faker _faker = new();

    public ServiceCollectionExtensionsTest()
    {
        _orchestratorConfiguration = new OrchestratorConfiguration()
        {
            CustomerProfileUpdateSubscriberConfiguration = new CustomerProfileUpdateSubscriberConfiguration(),
            CustomerPiiUpdateSubscriberConfiguration = new CustomerPiiUpdateSubscriberConfiguration(),
            SimIntelligencePublisherConfiguration = new SimIntelligencePublisherConfiguration(),
            IdentityIntelligencePublisherConfiguration = new IdentityIntelligencePublisherConfiguration(),
            EmailIntelligencePublisherConfiguration = new EmailIntelligencePublisherConfiguration()
        };

        var firestoreConfig = new FirestoreConfig
        {
            ProjectId = _faker.Random.Uuid().ToString(),
            EmulatorHost = _faker.Internet.Url()
        };

        _builder = WebApplication.CreateBuilder();
        _builder.Configuration.Bind("Orchestrator", _orchestratorConfiguration);

        _builder.Configuration["FirestoreConfig:ProjectId"] = firestoreConfig.ProjectId;
        _builder.Configuration["FirestoreConfig:EmulatorHost"] = firestoreConfig.EmulatorHost;
    }

    [Fact]
    public void RegisterServices_ShouldLoadPiiService_WhenSettingsIsConfiguredToUseSubscriberV2()
    {
        // Arrange
        const string expectedService =
            "Questrade.Library.PubSubClientHelper.Primitives.ISubscriberConfiguration`1[[Domain.Models.PubSubMessage`1[[Domain.Models.Pii.CustomerPiiUpdateMessage";
        _builder.Configuration[FeatureFlags.UseCustomerV2] = "true";



        // _builder.Configuration.GetSection(nameof(FirestoreConfig)).Get<FirestoreConfig>().Returns(_firestoreConfig);

        // Act
        _builder.RegisterServices(_orchestratorConfiguration);

        // Assert
        _builder.Services.Any(s => s.ServiceType.FullName!.Contains(expectedService)).Should().BeTrue();
    }

    [Fact]
    public void RegisterServices_ShouldLoadCustomerService_WhenSettingsIsConfiguredToUseSubscriberV1()
    {
        // Arrange
        const string expectedService =
            "Questrade.Library.PubSubClientHelper.Primitives.ISubscriberConfiguration`1[[Domain.Models.PubSubMessage`1[[Domain.Models.CustomerProfileUpdatedMessage";
        _builder.Configuration[FeatureFlags.UseCustomerV2] = "false";

        // Act
        _builder.RegisterServices(_orchestratorConfiguration);

        // Assert
        _builder.Services.Any(s => s.ServiceType.FullName!.Contains(expectedService)).Should().BeTrue();
    }
}
