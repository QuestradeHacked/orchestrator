using Domain.Constants;
using Domain.Models.CRM;
using Domain.Repositories.Firestore;
using Domain.Repositories.GraphQL;
using Domain.Services;
using GraphQL;
using GraphQL.Client.Abstractions;
using Infra.Constants;
using Infra.Repositories.GraphQL;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Unit.Faker;
using Xunit;

namespace Unit.Infra.Repositories.GraphQL;

public class CustomerRepositoryMetricTests
{

    private readonly ICustomerRepository _customerRepository;

    private readonly Bogus.Faker _faker = new();

    private readonly IGraphQLClient _graphQlClient;

    private readonly IMetricService _metricService;

    public CustomerRepositoryMetricTests()
    {
        var customerOnHoldRepository = Substitute.For<ICustomerOnHoldRepository>();
        _graphQlClient = Substitute.For<IGraphQLClient>();
        var logger = Substitute.For<ILogger<CustomerRepository>>();
        _metricService = Substitute.For<IMetricService>();

        _customerRepository = new CustomerRepository(
            _graphQlClient,
            logger,
            customerOnHoldRepository,
            _metricService
        );
    }

    [Fact]
    public async Task GetProfileAsync_ShouldDistributeCustomerRepositoryRequest_WhenFetchCrmCustomers()
    {
        //Arrange
        var dataPersonIds = _faker.Random.Number(1000,9999).ToString();
        var dataUserId = _faker.Random.Number(1000,9999).ToString();
        var tags = new List<string>
        {
            MetricTags.StatusSuccess,
            CustomerRepositoryMetricTags.ActionFetchCustomer
        };

        _graphQlClient
            .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
            .Returns(GraphQLResponseFaker.GenerateValidResponse());

        //Act
        await _customerRepository.GetProfileAsync(dataUserId, dataPersonIds);

        var result = Record.Exception(
            () => _metricService.Received().Distribution(
                Arg.Is<string>(MetricNames.CustomerRepositoryHandleRequest),
                Arg.Any<double>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProfileAsync_ShouldIncrementCustomerRepositoryRequest_WhenFailToFetchCrmCustomers()
    {
        //Arrange
        var dataPersonIds = _faker.Random.Number(1000,9999).ToString();
        var dataUserId = _faker.Random.Number(1000,9999).ToString();
        var tags = new List<string>
        {
            MetricTags.StatusPermanentError,
            CustomerRepositoryMetricTags.ActionFetchCustomer
        };

        _graphQlClient
            .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Test Exception"));

        //Act
        var result = await Record.ExceptionAsync(() =>
            _customerRepository.GetProfileAsync(dataUserId, dataPersonIds)
        );

        result = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.CustomerRepositoryHandleRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        //Assert
        Assert.Null(result);
    }
}
