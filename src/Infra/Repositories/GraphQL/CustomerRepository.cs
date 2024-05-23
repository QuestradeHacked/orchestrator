using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models.CRM;
using Domain.Repositories.Firestore;
using Domain.Repositories.GraphQL;
using Domain.Services;
using GraphQL;
using GraphQL.Client.Abstractions;
using Infra.Constants;
using Microsoft.Extensions.Logging;
using SerilogTimings;

namespace Infra.Repositories.GraphQL;

public class CustomerRepository : ICustomerRepository
{
    private const string ErrorPathForManage = "domesticaddress";

    private readonly ICustomerOnHoldRepository _customerOnHoldRepository;

    private readonly IGraphQLClient _graphQlClient;

    private readonly ILogger<CustomerRepository> _logger;

    private readonly IMetricService _metricService;

    public CustomerRepository(IGraphQLClient graphQlClient,
        ILogger<CustomerRepository> logger,
        ICustomerOnHoldRepository customerOnHoldRepository,
        IMetricService metricService
    )
    {
        _customerOnHoldRepository = customerOnHoldRepository;
        _graphQlClient = graphQlClient;
        _logger = logger;
        _metricService = metricService;
    }

    public async Task<Tuple<CrmPerson?, PersonAccount?>> GetProfileAsync(string dataUserId, string dataPersonIds,
        CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(dataUserId, out var userId))
        {
            throw new ArgumentException("UserId is not valid");
        }

        if (!int.TryParse(dataPersonIds, out var personIds))
        {
            throw new ArgumentException("PersonIds are not valid");
        }

        var queryResponse = await FetchCrmCustomer(userId, personIds, cancellationToken);

        if (queryResponse.Errors == null || !queryResponse.Errors.Any())
        {
            return Tuple.Create(
                queryResponse.Data.UserPerson?.FirstOrDefault()?.Persons?.FirstOrDefault(),
                queryResponse.Data.PersonAccounts?.Where(d => d.EffectiveDate.HasValue).MaxBy(d => d.EffectiveDate));
        }

        if (queryResponse.Errors.Any(error =>
                error.Path?.Any(path => Convert.ToString(path)?.ToLower() == ErrorPathForManage) == true))
        {
            _logger.LogDebug("Adding the customer {personId} to the hold list.", personIds);

            await _customerOnHoldRepository.UpsertAsync(new CustomerOnHold
            {
                PersonId = dataPersonIds,
                UserId = dataUserId,
                CreatedAt = DateTime.Now
            }, cancellationToken);
        }

        throw new CrmException(queryResponse.Errors.Select(error => new CrmException(error.Message)));
    }

    private async Task<GraphQLResponse<CrmResponse>> FetchCrmCustomer(int userId, int personIds, CancellationToken cancellationToken)
    {
        try
        {
            _metricService.Increment(
                MetricNames.CustomerRepositoryRequest,
                new List<string>{MetricTags.StatusSuccess}
            );

            var request = new GraphQLRequest(CrmConstants.GetUserAndPersonAccountsQuery, new { userId, personIds });
            var timing = Operation.Begin(
                "Fetching User information for {UserId} and Person Accounts for {PersonIds} from CRM.",
                userId,
                personIds
            );
            var queryResponse = await _graphQlClient.SendQueryAsync<CrmResponse>(request, cancellationToken);
            timing.Complete();

            _metricService.Distribution(
                MetricNames.CustomerRepositoryHandleRequest,
                timing.Elapsed.TotalMilliseconds,
                new List<string>
                {
                    MetricTags.StatusSuccess,
                    CustomerRepositoryMetricTags.ActionFetchCustomer
                }
            );

            return queryResponse;
        }
        catch(Exception)
        {
            _metricService.Increment(
                MetricNames.CustomerRepositoryHandleRequest,
                new List<string>
                {
                    MetricTags.StatusPermanentError,
                    CustomerRepositoryMetricTags.ActionFetchCustomer
                }
            );

            throw;
        }
    }
}
