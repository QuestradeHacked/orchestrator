using Domain.Models.Pii;

namespace Infra.Subscriber.Extensions.Pii;

public static class CustomerPiiRelatedReferenceExtension
{
    public static string? GetCrmUserId(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        var data = customerPiiUpdateMessage;

        return data?.Customer?.RelatedReferences.Where
        (
            r => r.Name?.ToLower() == "crm" &&
            r.AttributeName?.ToLower() == "userid"
        )
        .Select( id => id.Id )
        .FirstOrDefault();
    }
}
