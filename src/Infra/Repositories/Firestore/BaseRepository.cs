using Domain.Entities;
using Domain.Repositories.Firestore;
using Infra.Repositories.Firestore.Clients;
using System.Linq.Expressions;

namespace Infra.Repositories.Firestore;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly FirestoreClient<TEntity> FirestoreClient;

    protected BaseRepository(FirestoreClientFactory<TEntity> firestoreClientFactory, string collection)
    {
        FirestoreClient = firestoreClientFactory.Create(collection);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await FirestoreClient.ExistsByIdAsync(id, cancellationToken);
    }

    public async Task<List<TEntity>> GetByAsync(
        Expression<Func<TEntity, object>> property,
        object value,
        CancellationToken cancellationToken = default)
    {
        return await FirestoreClient.GetByAsync(property, value, cancellationToken);
    }

    public async Task<TEntity> GetSingleByAsync(
        Expression<Func<TEntity, object>> property,
        object value,
        CancellationToken cancellationToken = default)
    {
        var entities = await FirestoreClient.GetByAsync(property, value, cancellationToken);

        return entities.SingleOrDefault()!;
    }

    public virtual async Task<string> UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await FirestoreClient.UpsertAsync(entity, cancellationToken);

        return createdEntity.Id;
    }

    public async Task<bool> DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await FirestoreClient.DeleteAsync(id, cancellationToken);
    }
}
