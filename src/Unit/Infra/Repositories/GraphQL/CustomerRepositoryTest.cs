using Domain.Exceptions;
using Domain.Models.CRM;
using Domain.Repositories.Firestore;
using Domain.Services;
using GraphQL;
using GraphQL.Client.Abstractions;
using Infra.Repositories.GraphQL;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Unit.Faker;
using Xunit;

namespace Unit.Infra.Repositories.GraphQL
{
    public class CustomerRepositoryTest
    {
        private readonly CustomerRepository _customerRepository;

        private readonly ICustomerOnHoldRepository _customerOnHoldRepository;

        private readonly IGraphQLClient _graphQlClient;

        private readonly ILogger<CustomerRepository> _logger;

        private readonly IMetricService _metricService;

        public CustomerRepositoryTest()
        {
            _customerOnHoldRepository = Substitute.For<ICustomerOnHoldRepository>();
            _graphQlClient = Substitute.For<IGraphQLClient>();
            _logger = Substitute.For<ILogger<CustomerRepository>>();
            _metricService = Substitute.For<IMetricService>();

            _customerRepository = new CustomerRepository(
                _graphQlClient,
                _logger,
                _customerOnHoldRepository,
                _metricService
            );
        }

        [Fact]
        public async Task GetProfile_ShouldReturnValidCrmResponse_WhenPassAllParams()
        {
            // Arrange
            var dataUserId = new Random().Next().ToString();
            var dataPersonIds = new Random().Next().ToString();
            _graphQlClient
                .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
                .Returns(GraphQLResponseFaker.GenerateValidResponse());

            //Act
            var customerProfile = await _customerRepository.GetProfileAsync(dataUserId, dataPersonIds, CancellationToken.None);

            // Assert
            Assert.NotNull(customerProfile.Item1);
            Assert.NotNull(customerProfile.Item2);
        }

        [Fact]
        public async Task GetProfile_ShouldReturnAException_WhendataUserIdIsInvalid()
        {
            // Arrange
            var dataUserId = string.Empty;
            var dataPersonIds = new Random().Next().ToString();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _customerRepository.GetProfileAsync(dataUserId, dataPersonIds, CancellationToken.None));
        }

        [Fact]
        public async Task GetProfile_ShouldReturnAException_WhenDataPersonIdsIsInvalid()
        {
            // Arrange
            var dataUserId = new Random().Next().ToString();
            var dataPersonIds = string.Empty;

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _customerRepository.GetProfileAsync(dataUserId, dataPersonIds, CancellationToken.None));
        }

        [Fact]
        public async Task GetProfile_ShouldReturnAException_WhenCrmReturnError()
        {
            // Arrange
            var dataUserId = new Random().Next().ToString();
            var dataPersonIds = new Random().Next().ToString();
            _graphQlClient
                .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
                .Returns(GraphQLResponseFaker.GenerateInvalidResponse());

            // Assert
            await Assert.ThrowsAsync<CrmException>(() => _customerRepository.GetProfileAsync(dataUserId, dataPersonIds, CancellationToken.None));
        }

        [Fact]
        public async Task GetProfile_ShouldReturnAException_WhenCrmReturnAddressErrorMissingParamId()
        {
            // Arrange
            var dataUserId = new Random().Next().ToString();
            var dataPersonIds = new Random().Next().ToString();
            _graphQlClient
                .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
                .Returns(GraphQLResponseFaker.GenerateInvalidWithMissingParamIdForAddressQueryResponse());

            // Assert
            await Assert.ThrowsAsync<CrmException>(() => _customerRepository.GetProfileAsync(dataUserId, dataPersonIds, CancellationToken.None));
        }

        [Fact]
        public async Task GetProfile_ShouldReturnValidCrmResponseWithPersonAccountNull_WhenPassAllParams()
        {
            // Arrange
            var dataUserId = new Random().Next().ToString();
            var dataPersonIds = new Random().Next().ToString();
            _graphQlClient
                .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
                .Returns(GraphQLResponseFaker.GenerateEffectiveDateNullResponse());

            //Act
            var customerProfile = await _customerRepository.GetProfileAsync(dataUserId, dataPersonIds, CancellationToken.None);

            // Assert
            Assert.NotNull(customerProfile.Item1);
            Assert.Null(customerProfile.Item2);
        }

        [Fact]
        public async Task GetProfile_ShouldReturnValidCrmResponseWithPersonCrmPersonNull_WhenPassAllParams()
        {
            // Arrange
            var dataUserId = new Random().Next().ToString();
            var dataPersonIds = new Random().Next().ToString();
            _graphQlClient
                .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
                .Returns(GraphQLResponseFaker.GeneratePersonsNullResponse());

            //Act
            var customerProfile = await _customerRepository.GetProfileAsync(dataUserId, dataPersonIds, CancellationToken.None);

            // Assert
            Assert.Null(customerProfile.Item1);
            Assert.NotNull(customerProfile.Item2);
        }
    }
}
