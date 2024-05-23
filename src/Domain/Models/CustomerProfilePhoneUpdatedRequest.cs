using Domain.Models.Analysis;
using Domain.Models.CRM;

namespace Domain.Models;

public class CustomerProfilePhoneUpdatedRequest : CustomerPhoneAnalysisRequest, IRequestOrchestrator
{
    public void AddCustomerInfo(CrmPerson person, PersonAccount? personAccount)
    {
        if (!PhoneNumbers.Any())
        {
            PhoneNumbers = GetValidPhoneNumbers(person.PhoneNumbers);
        }

        if (personAccount == null)
        {
            return;
        }

        AccountDetailNumber = personAccount.AccountNumber;
        AccountStatusId = personAccount.AccountStatusId;
        EffectiveDate = personAccount.EffectiveDate;
    }

    private static IEnumerable<string> GetValidPhoneNumbers(IList<PhoneNumber>? phoneNumbers)
    {
        if (phoneNumbers is null)
        {
            return Enumerable.Empty<string>();
        }

        var validPhoneNumbers = from contact in phoneNumbers
            where !string.IsNullOrEmpty(contact.Number) && contact.Type == PhoneNumberType.Personal
            select contact.Number;

        return validPhoneNumbers.ToList()!;
    }
}
