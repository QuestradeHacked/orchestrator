using Domain.Models;

namespace Infra.Subscriber.Extensions;

public static class CustomerProfileUpdateMessageExtension
{
    public static string CustomerFirestoreId(this CustomerProfileUpdatedMessage data) => $"{data.PersonId}_{data.UserId}";
}
