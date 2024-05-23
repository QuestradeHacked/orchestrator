using Domain.Constants;
using Domain.Entities;
using Domain.Services;
using Google.Cloud.Firestore;
using SerilogTimings;
using System.Linq.Expressions;

namespace Infra.Repositories.Firestore.Clients;

public sealed class FirestoreClient<T> where T : BaseEntity
{
    private readonly IMetricService _metricService;

    private readonly CollectionReference _workingCollection;

    public FirestoreClient(
        string collectionName,
        FirestoreDb firestoreDb,
        IMetricService metricService
    )
    {
        _metricService = metricService;
        _workingCollection = firestoreDb?.Collection(collectionName)!;
    }

    public async Task<bool> ExistsByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var timing = StartMetricTimer(nameof(ExistsByIdAsync),$"id = {id}");
            var querySnapshot = await _workingCollection.Document(id).GetSnapshotAsync(cancellationToken);
            SendMetricTimer(timing,new List<string>{MetricTags.StatusSuccess});

            return querySnapshot?.Exists == true;
        }
        catch(Exception)
        {
            SendErrorMetric();

            throw;
        }
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var timing = StartMetricTimer(nameof(GetByIdAsync),$"id = {id}");
            var querySnapshot = await _workingCollection.Document(id).GetSnapshotAsync(cancellationToken);
            SendMetricTimer(timing, new List<string>{MetricTags.StatusSuccess});

            return querySnapshot.Exists ? querySnapshot.ConvertTo<T>() : default;
        }
        catch(Exception)
        {
            SendErrorMetric();

            throw;
        }
    }

    public async Task<List<T>> GetByAsync(Expression<Func<T, object>> property, object value,
        CancellationToken cancellationToken = default)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        QuerySnapshot querySnapshot;
        var propertyName = ((MemberExpression)property.Body).Member.Name;
        List<T> result;

        try
        {
            var timing = StartMetricTimer(nameof(GetByIdAsync));
            querySnapshot = await _workingCollection.WhereEqualTo(propertyName, value)
                .GetSnapshotAsync(cancellationToken);
            SendMetricTimer(timing,new List<string>{MetricTags.StatusSuccess});
        }
        catch(Exception)
        {
            SendErrorMetric();

            throw;
        }

        result = querySnapshot.Documents.Select(x => x.ConvertTo<T>()).ToList();

        return await ClearDuplicates(result, cancellationToken);
    }

    public async Task<T> UpsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            DocumentReference document;
            Operation timing;

            if (!string.IsNullOrWhiteSpace(entity.Id))
            {
                document = _workingCollection.Document(entity.Id);
                timing = StartMetricTimer(nameof(UpsertAsync), $"Update id = ${entity.Id}");
                await document.SetAsync(entity, SetOptions.Overwrite, cancellationToken);
                SendMetricTimer(timing, new List<string>{MetricTags.StatusSuccess});
            }
            else
            {
                timing = StartMetricTimer(nameof(UpsertAsync), $"Insert");
                document = await _workingCollection.AddAsync(entity, cancellationToken);
                SendMetricTimer(timing, new List<string>{MetricTags.StatusSuccess});
            }

            timing = StartMetricTimer(nameof(UpsertAsync), $"GetSnapShotAsync id = {document.Id}");
            var entitySnapshot = await document.GetSnapshotAsync(cancellationToken);
            SendMetricTimer(timing, new List<string>{MetricTags.StatusSuccess});

            return entitySnapshot.ConvertTo<T>();
        }
        catch(Exception)
        {
            SendErrorMetric();

            throw;
        }
    }

    private async Task<List<T>> ClearDuplicates(IEnumerable<T> source, CancellationToken cancellationToken)
    {
        var distinctItems = new List<T>();
        var tasks = new List<Task>();

        foreach (var pair in source.GroupBy(e => e))
        {
            distinctItems.Add(pair.Key);
            foreach (var duplicate in pair)
            {
                if (!duplicate.Equals(pair.Key))
                {
                    tasks.Add(DeleteAsync(duplicate.Id, cancellationToken));
                }
            }
        }

        await Task.WhenAll(tasks);
        return distinctItems;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var timing = StartMetricTimer(nameof(DeleteAsync), $"Delete id = ${id}");
            await _workingCollection.Document(id).DeleteAsync(cancellationToken: cancellationToken);
            SendMetricTimer(timing,new List<string>{MetricTags.StatusSuccess});

            return await Task.FromResult(true);
        }
        catch (Exception)
        {
            SendErrorMetric();

            return await Task.FromResult(false);
        }
    }

    private Operation StartMetricTimer(string functionName, string? message=null)
    {
        _metricService.Increment(
            MetricNames.FirestoreRequest,
            new List<string>{MetricTags.StatusSuccess}
        );

        return Operation.Begin(
            "{Collection}-{FunctionName}: {Message}",
            _workingCollection.Id,
            functionName,
            message!
        );
    }

    private void SendMetricTimer(Operation timing,List<string> tags)
    {
        timing.Complete();

        _metricService.Distribution(
            MetricNames.FirestoreHandleRequest,
            timing.Elapsed.TotalMilliseconds,
            tags
        );
    }

    private void SendErrorMetric()
    {
         _metricService.Increment(
            MetricNames.FirestoreHandleRequest,
            new List<string>{MetricTags.StatusPermanentError}
        );
    }
}
