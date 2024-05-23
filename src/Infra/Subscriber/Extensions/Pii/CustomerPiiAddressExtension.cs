using Domain.Models.Pii;

namespace Infra.Subscriber.Extensions.Pii;

public static class CustomerPiiAddressExtension
{
    public static CustomerPiiAddress? GetPrimaryAddress(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        return customerPiiUpdateMessage?.Customer?.Profile?.Personal?.Addresses?.Where
        (
            a =>a.Status?.ToLower() == "active" &&
            a.Type?.ToLower() == "primary"
        ).FirstOrDefault();
    }

    public static bool HasPrimaryAddress(
        this CustomerPiiUpdateMessage customerPiiUpdateMessage
    )
    {
        var data = customerPiiUpdateMessage;

        return data?.Customer?.Profile?.Personal?.Addresses?.Exists
        (
            a => a.Type?.ToLower() == "primary" &&
            a.Status?.ToLower() == "active"
        ) ?? false;
    }

    public static bool HasPrimaryAddressCreated(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        var data = customerPiiUpdateMessage;

        return data?.Customer?.Profile?.Personal?.Addresses?.Exists
        (
            a => !data.DeltaChanges.Any() &&
                a.Type?.ToLower() == "primary" &&
                a.Status?.ToLower() == "active"
        ) ?? false;
    }
}
