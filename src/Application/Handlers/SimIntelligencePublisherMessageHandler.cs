using Domain.Configuration;
using Domain.Models;
using Domain.Models.Analysis;
using Infra.Util;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Questrade.Library.PubSubClientHelper.Primitives;
using Questrade.Library.PubSubClientHelper.Publisher;

namespace Application.Handlers;

public class SimIntelligencePublisherMessageHandler : AsyncRequestHandler<CustomerPhoneAnalysisRequest>
{
    private readonly IFeatureManager _featureManager;

    private readonly ILogger<SimIntelligencePublisherMessageHandler> _logger;

    private readonly IPublisherService<SimIntelligencePublisherMessage> _publisherService;

    private readonly IServiceProvider _serviceProvider;

    public SimIntelligencePublisherMessageHandler(ILogger<SimIntelligencePublisherMessageHandler> logger,
        IServiceProvider serviceProvider, IFeatureManager featureManager)
    {
        _featureManager = featureManager;
        _logger = logger;
        _publisherService = serviceProvider.GetRequiredService<IPublisherService<SimIntelligencePublisherMessage>>();
        _serviceProvider = serviceProvider;
        _featureManager = featureManager;
    }

    protected override async Task Handle(CustomerPhoneAnalysisRequest request, CancellationToken cancellationToken)
    {
        if (!await _featureManager.IsEnabledAsync(FeatureFlags.PublishSimIntelligenceEventsFeature))
        {
            _logFeatureFlagOffInformation(_logger, FeatureFlags.PublishSimIntelligenceEventsFeature, null);
            return;
        }

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var correlationContext = scope.ServiceProvider.GetRequiredService<CorrelationContext>();

            foreach (var phoneNumber in request.PhoneNumbers)
            {
                await _publisherService.PublishMessageAsync(new SimIntelligencePublisherMessage
                {
                    AccountNumber = request.AccountDetailNumber,
                    AccountStatusId = request.AccountStatusId,
                    CrmUserId = request.CrmUserId,
                    EffectiveDate = request.EffectiveDate,
                    PhoneNumber = phoneNumber,
                    ProfileId = request.ProfileId
                }, new PublishingOptions { TrackingId = correlationContext.CorrelationId });
            }

            _logPhonePublishedInformation(_logger, request.CrmUserId, null);
        }
        catch (Exception error)
        {
            _logSimIntelligenceError(_logger, request.CrmUserId, error);
        }
    }

    private readonly Action<ILogger, string?, Exception?> _logPhonePublishedInformation =
            LoggerMessage.Define<string?>(
                eventId: new EventId(1, nameof(SimIntelligencePublisherMessageHandler)),
                formatString: "Published all phones numbers from {CrmUserId}",
                logLevel: LogLevel.Information
            );
    private readonly Action<ILogger, string?, Exception?> _logSimIntelligenceError = LoggerMessage.Define<string?>(
        eventId: new EventId(2, nameof(SimIntelligencePublisherMessageHandler)),
        formatString: "Failed on publish an Phone from {CrmUserId}",
        logLevel: LogLevel.Warning
    );
    private readonly Action<ILogger, string?, Exception?> _logFeatureFlagOffInformation = LoggerMessage.Define<string?>(
        eventId: new EventId(3, nameof(EmailIntelligencePublisherMessageHandler)),
        formatString: "Feature flag {featureFlag} is off.",
        logLevel: LogLevel.Information
    );
}
