using Domain.Entities;
using Domain.Repositories.Firestore;
using Infra.Repositories.Firestore.Clients;
using Microsoft.Extensions.Logging;

namespace Infra.Repositories.Firestore;

public class CustomerOnHoldRepository : BaseRepository<CustomerOnHold>, ICustomerOnHoldRepository
{
    private readonly ILogger<CustomerOnHoldRepository> _logger;

    public CustomerOnHoldRepository(
        FirestoreClientFactory<CustomerOnHold> firestoreClientFactory,
        ILogger<CustomerOnHoldRepository> logger)
        : base(firestoreClientFactory, nameof(CustomerOnHold))
    {
        _logger = logger;
    }

    public override async Task<string> UpsertAsync(CustomerOnHold entity, CancellationToken cancellationToken = default)
    {
        var isStoredCustomerOnHold =
            await ExistsAsync(entity.Id, cancellationToken);
        if (isStoredCustomerOnHold)
        {
            return entity.Id;
        }

        await base.UpsertAsync(entity, cancellationToken);

        return entity.UserId;
    }

    public async Task<bool> ReleaseCustomerOnHoldAsync(string id, CancellationToken cancellationToken)
    {
        var isStoredCustomerOnHold =
            await ExistsAsync(id, cancellationToken);
        if (!isStoredCustomerOnHold)
        {
            return true;
        }
        return await DeleteByIdAsync(id, cancellationToken);
    }
}
