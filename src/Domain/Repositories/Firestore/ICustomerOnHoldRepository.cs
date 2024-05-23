using Domain.Entities;

namespace Domain.Repositories.Firestore;

public interface ICustomerOnHoldRepository : IBaseRepository<CustomerOnHold>
{
    Task<bool> ReleaseCustomerOnHoldAsync(string id, CancellationToken cancellationToken);
}
