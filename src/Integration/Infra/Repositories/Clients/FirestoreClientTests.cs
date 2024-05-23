using Domain.Constants;
using Domain.Entities;
using Domain.Services;
using Infra.Repositories.Firestore.Clients;
using Integration.Faker;
using Integration.Fixture;
using NSubstitute;
using Xunit;

namespace Integration.Infra.Repositories.Clients;

public class FirestoreTests:IAssemblyFixture<FirestoreEmulatorFixture>
{
    private readonly Bogus.Faker _faker = new();

    private readonly FirestoreClient<CustomerOnHold> _fireStoreClient;

    private readonly IMetricService _metricService;

    public FirestoreTests(FirestoreEmulatorFixture firestoreEmulatorFixture)
    {

        var firestoreDb = firestoreEmulatorFixture.CreateFirestoreDb();
        _metricService = Substitute.For<IMetricService>();

        _fireStoreClient = new FirestoreClient<CustomerOnHold>(
            _faker.Random.Word(),
            firestoreDb,
            _metricService
        );
    }

    [Fact]
    public async Task ExistsByIdAsync_ShouldIncrementAndDistributeMetrics_WhenSuccess ()
    {
        // Arrange
        var id = _faker.Random.Number(1000,9999).ToString();
        var tags = new List<string>{MetricTags.StatusSuccess};

        // Act
        await _fireStoreClient.ExistsByIdAsync(id);

        var resultIncrement = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        var resultDistribution  = Record.Exception(
            () => _metricService.Received().Distribution(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Any<double>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task ExistsByIdAsync_ShouldIncrementCountAndErrorMetrics_WhenFail()
    {
        // Arrange
        var id = string.Empty;
        var tagFail = new List<string> { MetricTags.StatusPermanentError };
        var tagSuccess = new List<string> { MetricTags.StatusSuccess };

        // Act
        var result = await Record.ExceptionAsync(
            () => _fireStoreClient.ExistsByIdAsync(id)
        );

        var resultIncrement = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagSuccess))
            )
        );

        var resultDistribution = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagFail))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncrementAndDistributeMetrics_WhenSuccess ()
    {
        // Arrange
        var id = _faker.Random.Number(1000,9999).ToString();
        var tags = new List<string>{MetricTags.StatusSuccess};

        // Act
        await _fireStoreClient.GetByIdAsync(id);

        var resultIncrement = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        var resultDistribution  = Record.Exception(
            () => _metricService.Received().Distribution(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Any<double>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncrementCountAndErrorMetrics_WhenFail()
    {
        // Arrange
        var id = string.Empty;
        var tagFail = new List<string> { MetricTags.StatusPermanentError };
        var tagSuccess = new List<string> { MetricTags.StatusSuccess };

        // Act
        var result = await Record.ExceptionAsync(
            () => _fireStoreClient.GetByIdAsync(id)
        );

        var resultIncrement = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagSuccess))
            )
        );

        var resultDistribution = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagFail))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task GetByAsync_ShouldIncrementAndDistributeMetrics_WhenSuccess ()
    {
        // Arrange
        var id = _faker.Random.Number(1000,9999).ToString();
        var tags = new List<string>{MetricTags.StatusSuccess};

        // Act
        await _fireStoreClient.GetByAsync((d) => d.Id, id);

        var resultIncrement = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        var resultDistribution  = Record.Exception(
            () => _metricService.Received().Distribution(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Any<double>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task GetByAsync_ShouldIncrementCountAndErrorMetrics_WhenFail()
    {
        // Arrange
        var id = new Exception("Test Exception");
        var tagFail = new List<string> { MetricTags.StatusPermanentError };
        var tagSuccess = new List<string> { MetricTags.StatusSuccess };

        // Act
        var result = await Record.ExceptionAsync(
            () => _fireStoreClient.GetByAsync((d) => d.UserId,id)
        );

        var resultIncrement = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagSuccess))
            )
        );

        var resultDistribution = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagFail))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task UpSertByAsync_ShouldIncrementAndDistributeMetrics_WhenReceiveNewRecord ()
    {
        // Arrange
        var entity = CustomerOnHoldFaker.GenerateNoIdCustomerOnHold();
        var tags = new List<string>{MetricTags.StatusSuccess};

        // Act
        await _fireStoreClient.UpsertAsync(entity);

        var resultIncrement = Record.Exception(
            () => _metricService.Received(2).Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        var resultDistribution  = Record.Exception(
            () => _metricService.Received(2).Distribution(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Any<double>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task UpSertByAsync_ShouldIncrementAndDistributeMetrics_WhenUpdateRecord ()
    {
        // Arrange
        var entity = CustomerOnHoldFaker.GenerateValidCustomerOnHold();
        var tags = new List<string>{MetricTags.StatusSuccess};

        // Act
        await _fireStoreClient.UpsertAsync(entity);

        var resultIncrement = Record.Exception(
            () => _metricService.Received(2).Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        var resultDistribution  = Record.Exception(
            () => _metricService.Received(2).Distribution(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Any<double>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task UpSertAsync_ShouldDistributedErrorMetrics_WhenFail()
    {
        // Arrange
        var tagFail = new List<string> { MetricTags.StatusPermanentError };
        var tagSuccess = new List<string> { MetricTags.StatusSuccess };

        // Act
        var result = await Record.ExceptionAsync(
            () => _fireStoreClient.UpsertAsync(null!)
        );

        var resultDistribution = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagFail))
            )
        );

        // Assert
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task DeleteAsync_ShouldIncrementAndDistributeMetrics_WhenSuccess ()
    {
        // Arrange
        var id = _faker.Random.Number(1000,9999).ToString();
        var tags = new List<string>{MetricTags.StatusSuccess};

        // Act
        await _fireStoreClient.DeleteAsync(id,new CancellationToken());

        var resultIncrement = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        var resultDistribution  = Record.Exception(
            () => _metricService.Received().Distribution(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Any<double>(),
                Arg.Is<List<string>>(l => l.SequenceEqual(tags))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }

    [Fact]
    public async Task DeleteAsync_ShouldIncrementAndErrorMetrics_WhenFail()
    {
        // Arrange
        var id = string.Empty;
        var tagFail = new List<string> { MetricTags.StatusPermanentError };
        var tagSuccess = new List<string> { MetricTags.StatusSuccess };

        // Act
        var result = await Record.ExceptionAsync(
            () => _fireStoreClient.DeleteAsync(id, default)
        );

        var resultIncrement = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is<string>(MetricNames.FirestoreRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagSuccess))
            )
        );

        var resultDistribution = Record.Exception(
            () => _metricService.Received().Increment(
                Arg.Is(MetricNames.FirestoreHandleRequest),
                Arg.Is<List<string>>(l => l.SequenceEqual(tagFail))
            )
        );

        // Assert
        Assert.Null(resultIncrement);
        Assert.Null(resultDistribution);
    }
}
