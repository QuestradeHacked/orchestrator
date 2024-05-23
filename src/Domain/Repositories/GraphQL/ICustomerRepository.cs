using Domain.Models.CRM;

namespace Domain.Repositories.GraphQL;

public interface ICustomerRepository
{
    Task<Tuple<CrmPerson?, PersonAccount?>> GetProfileAsync(string dataUserId, string dataPersonIds,
        CancellationToken cancellationToken = default);
}
