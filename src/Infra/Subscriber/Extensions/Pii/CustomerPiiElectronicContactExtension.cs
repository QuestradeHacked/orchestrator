using Domain.Models.Analysis;
using Domain.Models.Pii;

namespace Infra.Subscriber.Extensions.Pii;

public static class CustomerPiiElectronicContactExtension
{
     private static readonly IList<string> ValidOperations = new List<string>{ "add", "replace" };

    public static CustomerEmailAnalysisRequest? EmailUpdateHandler(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        return !customerPiiUpdateMessage.HasPrimaryEmail() ? null :
            customerPiiUpdateMessage.ToCustomerEmailAnalysisRequest();
    }
    public static bool HasPrimaryEmailChanges(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        var data = customerPiiUpdateMessage;
        var electronicContacts = data.Customer!.Profile!.Personal!.ElectronicContacts;

        return electronicContacts.Exists(e =>
            (
                !data.DeltaChanges.Any()
                && e.Type?.ToLower() =="email" &&
                e.Subtype?.ToLower() == "email" &&
                e.Status?.ToLower() == "active"
            ) ||
            data.DeltaChanges.Exists
            (
                d =>
                d.Path!.ToLower() == "/profile/personal/electroniccontacts" &&
                ValidOperations.Contains(d.Operation!.ToLower()) &&
                d.GetValueAsDeltaChangeValues<List<CustomerPiiElectronicContact>>()!.Exists
                (
                    v => v.Status?.ToLower() == "active" &&
                    v.Type?.ToLower() == "email" &&
                    v.Subtype?.ToLower() == "email"
                )
            )
        );
    }

    public static CustomerPiiElectronicContact? GetPrimaryEmail(this CustomerPiiUpdateMessage
        customerPiiUpdateMessage)
    {
        var data = customerPiiUpdateMessage;

        return data.Customer!.Profile!.Personal!.ElectronicContacts?.Where(e =>
                e.Type?.ToLower() =="email" &&
                e.Subtype?.ToLower() == "email" &&
                e.Status?.ToLower() == "active")
            .FirstOrDefault();
    }

    public static bool HasPrimaryEmail(this CustomerPiiUpdateMessage
        customerPiiUpdateMessage)
    {
        var data = customerPiiUpdateMessage;

        return data.Customer!.Profile!.Personal!.ElectronicContacts?.Exists
        (
            e => e.Type?.ToLower() =="email" &&
                e.Subtype?.ToLower() == "email" &&
                e.Status?.ToLower() == "active"
        ) ?? false;
    }
}
