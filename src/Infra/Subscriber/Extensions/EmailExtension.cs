using Domain.Models;
using Domain.Models.Pii;

namespace Infra.Subscriber.Extensions;

public static class EmailExtension
{
    public static IRequestOrchestrator? HandleEmailChanges(this CustomerProfileUpdatedMessage data)
    {
        var emailChanged = data.Email;

        if (string.IsNullOrEmpty(emailChanged))
        {
            return null;
        }

        return new CustomerProfileEmailUpdatedRequest
        {
            CrmUserId = data.UserId,
            Email = data.Email!,
            ProfileId = data.PersonId,
        };
    }
}
