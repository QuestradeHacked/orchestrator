using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories.Firestore;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<bool> ExistsAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<TEntity> GetSingleByAsync(
        Expression<Func<TEntity, object>> property,
        object value,
        CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetByAsync(
        Expression<Func<TEntity, object>> property,
        object value,
        CancellationToken cancellationToken = default);

    Task<string> UpsertAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(
        string id,
        CancellationToken cancellationToken = default);
}
