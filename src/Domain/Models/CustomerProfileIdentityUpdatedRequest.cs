using DateTime = System.DateTime;
using Domain.Models.CRM;
using System.Globalization;
using Domain.Models.Analysis;

namespace Domain.Models;

public class CustomerProfileIdentityUpdatedRequest : CustomerIdentityAnalysisRequest, IRequestOrchestrator
{
    public void AddCustomerInfo(CrmPerson person, PersonAccount? personAccount)
    {
        if (!PhoneNumbers.Any())
        {
            PhoneNumbers = GetValidPhoneNumbers(person.PhoneNumbers);
        }

        DateOfBirth = FormatBirthDate(person.BirthDate);
        FirstName = person.FirstName;
        LastName = person.LastName;

        if (personAccount == null)
        {
            return;
        }

        AccountDetailNumber = personAccount.AccountNumber;
        AccountStatusId = personAccount.AccountStatusId;
        EffectiveDate = personAccount.EffectiveDate;

        if (person.DomesticAddress is not null)
        {
            AddressLine1 = FormatStreet(person.DomesticAddress);
            AddressCountryCode = "CA";
            City = person.DomesticAddress.City;
            PostalCode = person.DomesticAddress.PostalCode;
            State = person.DomesticAddress.Province;
        }
        else
        {
            AddressLine1 = person.InternationalAddress!.AddressLine1;
            AddressLine2 = person.InternationalAddress!.AddressLine2;
            AddressCountryCode = person.InternationalAddress!.CountryCode;
            PostalCode = person.InternationalAddress!.PostalCode;
            State = person.InternationalAddress!.ProvinceState;
        }
    }

    private static string FormatBirthDate(string? personBirthDate)
    {
        return personBirthDate is null ? string.Empty : DateTime.Parse(personBirthDate, CultureInfo.CurrentCulture).ToString("yyyyMMdd");
    }

    private static string FormatStreet(DomesticAddress domesticAddress)
    {
        return $"{domesticAddress.StreetNumber} {domesticAddress.StreetName} {domesticAddress.StreetType} {domesticAddress.StreetDirection}";
    }

    private static IEnumerable<string> GetValidPhoneNumbers(IList<CRM.PhoneNumber>? phoneNumbers)
    {
        if (phoneNumbers is null)
        {
            return Enumerable.Empty<string>();
        }

        var validPhoneNumbers = from contact in phoneNumbers
            where !string.IsNullOrEmpty(contact.Number) && contact.Type == PhoneNumberType.Personal
            select contact.Number;

        return validPhoneNumbers.ToList();
    }
}
