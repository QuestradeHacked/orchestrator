using Domain.Models;
using Domain.Models.Pii;

namespace Infra.Subscriber.Extensions;

public static class PhoneNumbersExtension
{
    public static IRequestOrchestrator? HandlePhoneChangesForIdentityIntelligence(this CustomerProfileUpdatedMessage? data)
    {
        var validPhoneNumbers = GetValidPhoneNumbers(data);

        if (!validPhoneNumbers.Any())
        {
            return null;
        }

        var phoneUpdatedRequest = new CustomerProfileIdentityUpdatedRequest
        {
            CrmUserId = data?.UserId,
            PhoneNumbers = validPhoneNumbers
        };

        return phoneUpdatedRequest;
    }

    public static IRequestOrchestrator? HandlePhoneChangesForSimIntelligence(this CustomerProfileUpdatedMessage? data)
    {
        var validPhoneNumbers = GetValidPhoneNumbers(data);

        if (!validPhoneNumbers.Any())
        {
            return null;
        }

        var phoneUpdatedRequest = new CustomerProfilePhoneUpdatedRequest
        {
            CrmUserId = data?.UserId,
            PhoneNumbers = validPhoneNumbers!
        };

        return phoneUpdatedRequest;
    }

    private static IList<string> GetValidPhoneNumbers(CustomerProfileUpdatedMessage? data)
    {
        if (data?.PhoneNumbers is null)
        {
            return new List<string>();
        }

        var phoneNumbers = from contact in data.PhoneNumbers ?? new List<CustomerPhoneNumber>()
            where !string.IsNullOrEmpty(contact.PhoneNumber) && contact.PhoneNumberType == PhoneNumberType.Personal
            select contact.PhoneNumber;

        return phoneNumbers.ToList()!;
    }
}
