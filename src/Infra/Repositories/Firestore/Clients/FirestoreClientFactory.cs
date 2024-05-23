using Domain.Entities;
using Domain.Services;
using Google.Cloud.Firestore;

namespace Infra.Repositories.Firestore.Clients;

public class FirestoreClientFactory<TEntity> where TEntity : BaseEntity
{
    private readonly FirestoreDb _firestoreDb;

    private readonly IMetricService _metricService;

    public FirestoreClientFactory(FirestoreDb firestoreDb, IMetricService metricService)
    {
        _firestoreDb = firestoreDb;
        _metricService = metricService;
    }

    public FirestoreClient<TEntity> Create(string collectionName)
    {
        var client = new FirestoreClient<TEntity>(
            collectionName,
            _firestoreDb,
            _metricService
        );

        return client;
    }
}
