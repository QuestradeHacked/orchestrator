using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories.Firestore;
using Domain.Repositories.GraphQL;
using Domain.Services;
using Google.Cloud.Firestore;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Grpc.Core;
using Infra.Config;
using Infra.Config.PubSub;
using Infra.Repositories.Firestore;
using Infra.Repositories.Firestore.Clients;
using Infra.Repositories.GraphQL;
using Infra.Services;
using Infra.Subscriber;
using Infra.Util;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using Orchestrator.Config;
using Questrade.Library.HealthCheck.AspNetCore.Extensions;
using Questrade.Library.PubSubClientHelper.Extensions;
using Questrade.Library.PubSubClientHelper.HealthCheck;
using Questrade.Library.PubSubClientHelper.Primitives;
using Questrade.Library.PubSubClientHelper.Publisher.Outbox;
using Serilog;
using StatsdClient;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using Domain.Configuration;
using Domain.Models.Pii;

namespace Orchestrator.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder,
        OrchestratorConfiguration orchestratorConfiguration)
    {
        builder.AddQuestradeHealthCheck();
        builder.Host.UseSerilog((context, logConfiguration) =>
            logConfiguration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddControllers();
        builder.Services
            .AddSubscribers(orchestratorConfiguration, builder)
            .AddPublisher(orchestratorConfiguration)
            .AddDataDogMetrics(builder.Configuration)
            .AddFirestore(builder.Configuration)
            .AddRepositories()
            .AddAppServices()
            .AddCrmService(builder.Configuration)
            .AddCorrelationContext()
            .AddFeatureManagement();

        builder.Services.AddMediatR
        (
            AppDomain.CurrentDomain.Load("Orchestrator"),
            AppDomain.CurrentDomain.Load("Application"),
            AppDomain.CurrentDomain.Load("Infra")
        );

        return builder;
    }

    private static IServiceCollection AddDataDogMetrics(this IServiceCollection services, IConfiguration configuration)
    {
        var dDMetricsConfig = configuration.GetSection("DataDog:StatsD").Get<DataDogMetricsConfig>();

        services.AddSingleton<IDogStatsd>(_ =>
        {
            var statsdConfig = new StatsdConfig
            {
                Prefix = dDMetricsConfig.Prefix,
                StatsdServerName = dDMetricsConfig.HostName
            };

            var dogStatsdService = new DogStatsdService();
            dogStatsdService.Configure(statsdConfig);

            return dogStatsdService;
        });

        return services;
    }

    private static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        // Services
        services.AddTransient<IMetricService, MetricService>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();

        return services;
    }

    private static IServiceCollection AddCrmService(this IServiceCollection services, IConfiguration configuration)
    {
        var crmConfig = configuration.GetSection("Crm").Get<CrmConfig>();

        services.AddSingleton<IGraphQLClient>(provider =>
        {
            ValidateToken(crmConfig.Token, provider.GetService<IHostEnvironment>()?.EnvironmentName);

            var gqlHttpClient = CreateGraphQlHttpClient(crmConfig);
            return gqlHttpClient;
        });

        return services;
    }

    private static void ValidateToken(string token, string? environmentName)
    {
        if (!IsTokenValid(token, environmentName))
        {
            throw new InvalidOperationException("Authorization Token is required to target non-development environment.");
        }
    }

    private static GraphQLHttpClient CreateGraphQlHttpClient(CrmConfig crmConfig)
    {
        var gqlHttpClientOptions = new GraphQLHttpClientOptions
        {
            EndPoint = new Uri(crmConfig.Endpoint)
        };

        var gqlHttpClient = new GraphQLHttpClient(gqlHttpClientOptions, new NewtonsoftJsonSerializer());

        if (!string.IsNullOrWhiteSpace(crmConfig.Token))
        {
            gqlHttpClient.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", crmConfig.Token);
        }

        return gqlHttpClient;
    }

    private static IServiceCollection AddSubscribers(this IServiceCollection services,
        OrchestratorConfiguration configuration, WebApplicationBuilder builder)
    {
        var subscribeToCustomerV2Str = builder.Configuration[FeatureFlags.UseCustomerV2];

        var subscribeToCustomerV2 = Convert.ToBoolean(subscribeToCustomerV2Str);

        if (!subscribeToCustomerV2)
        {
            services.RegisterSubscriber<
                CustomerProfileUpdateSubscriberConfiguration,
                PubSubMessage<CustomerProfileUpdatedMessage>,
                CustomerProfileUpdateSubscriber,
                SubscriberHealthCheck<
                    CustomerProfileUpdateSubscriberConfiguration,
                    PubSubMessage<CustomerProfileUpdatedMessage>
                >
            >(configuration.CustomerProfileUpdateSubscriberConfiguration!);
        }
        else
        {
            services.RegisterSubscriber<
                CustomerPiiUpdateSubscriberConfiguration,
                PubSubMessage<CustomerPiiUpdateMessage>,
                CustomerPiiUpdateSubscriber,
                SubscriberHealthCheck<
                    CustomerPiiUpdateSubscriberConfiguration,
                    PubSubMessage<CustomerPiiUpdateMessage>
                >
            >(configuration.CustomerPiiUpdateSubscriberConfiguration!);
        }

        return services;
    }

    private static IServiceCollection AddPublisher(this IServiceCollection services,
        OrchestratorConfiguration configuration)
    {
        RegisterOutboxPublisherWithInMemoryOutbox<SimIntelligencePublisherConfiguration,
            SimIntelligencePublisherMessage>(services, configuration.SimIntelligencePublisherConfiguration!);

        RegisterOutboxPublisherWithInMemoryOutbox<IdentityIntelligencePublisherConfiguration,
            IdentityIntelligencePublisherMessage>(services, configuration.IdentityIntelligencePublisherConfiguration!);

        RegisterOutboxPublisherWithInMemoryOutbox<EmailIntelligencePublisherConfiguration,
            EmailIntelligencePublisherMessage>(services, configuration.EmailIntelligencePublisherConfiguration!);

        return services;
    }

    private static bool IsTokenValid(string token, string? environmentName)
    {
        var environmentsRequired = new[] { QtEnvironments.UAT, QtEnvironments.Production };

        return !environmentsRequired.Contains(environmentName) || !string.IsNullOrWhiteSpace(token);
    }

    private static void RegisterOutboxPublisherWithInMemoryOutbox<TConfiguration, TPublisher>(
        IServiceCollection services, TConfiguration configuration)
        where TPublisher : class, new()
        where TConfiguration : class, IPublisherConfiguration<TPublisher>
    {
        services.RegisterOutboxPublisherWithInMemoryOutbox<
            TConfiguration,
            TPublisher,
            OutboxPubsubPublisherService<TConfiguration, TPublisher>,
            PubsubPublisherBackgroundService<TConfiguration, TPublisher>,
            PublisherHealthCheck<TConfiguration, TPublisher>
        >(configuration);
    }

    private static IServiceCollection AddCorrelationContext(this IServiceCollection services)
    {
        services.AddScoped<CorrelationContext>();

        return services;
    }

    private static IServiceCollection AddFirestore(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection(nameof(FirestoreConfig)).Get<FirestoreConfig>();

        if (string.IsNullOrWhiteSpace(config.ProjectId))
        {
            throw new ProjectIdIsEmptyException();
        }

        services.AddSingleton(_ =>
        {
            var builder = new FirestoreDbBuilder
            {
                ProjectId = config.ProjectId
            };

            if (string.IsNullOrWhiteSpace(config.EmulatorHost))
            {
                return builder.Build();
            }

            builder.ChannelCredentials = ChannelCredentials.Insecure;
            builder.Endpoint = config.EmulatorHost;

            return builder.Build();
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.TryAddSingleton(typeof(FirestoreClientFactory<>));
        services.TryAddSingleton<ICustomerOnHoldRepository, CustomerOnHoldRepository>();
        return services;
    }
}
