using Domain.Exceptions;
using Domain.Models.Analysis;
using Domain.Models.CRM;

namespace Domain.Models;

public class CustomerProfileEmailUpdatedRequest : CustomerEmailAnalysisRequest, IRequestOrchestrator
{
    public void AddCustomerInfo(CrmPerson person, PersonAccount? personAccount)
    {
        if (person.InternationalAddress is null && person.DomesticAddress is null)
        {
            throw new NoAddressFoundException();
        }

        CreatedAt = person.Created;
        EnterpriseProfileId = "";
        FirstName = person.FirstName;
        LastName = person.LastName;
        PhoneNumber = GetPersonalPhoneNumber(person);
        UpdatedAt = person.Updated;

        if (string.IsNullOrEmpty(Email))
        {
            Email = person.Email!;
        }

        if (person.DomesticAddress is not null)
        {
            City = person.DomesticAddress.City;
            Country = "Canada";
            State = person.DomesticAddress.Province;
            Street = person.DomesticAddress.StreetName;
            ZipCode = person.DomesticAddress.PostalCode;
        }
        else
        {
            City = person.InternationalAddress!.AddressLine2;
            Country = person.InternationalAddress.CountryName;
            State = person.InternationalAddress.ProvinceState;
            Street = person.InternationalAddress.AddressLine1;
            ZipCode = person.InternationalAddress.PostalCode;
        }

        if (personAccount != null)
        {
            AccountDetailNumber = personAccount.AccountNumber;
            AccountStatusId = personAccount.AccountStatusId;
            EffectiveDate = personAccount.EffectiveDate;
        }
    }

    private static string GetPersonalPhoneNumber(CrmPerson person)
    {
        var personalNumbers = person.PhoneNumbers.Where(p => p.Type == PhoneNumberType.Personal);

        if (!personalNumbers.Any())
        {
            throw new PersonalPhoneNumberIsMissingException();
        }

        return person.PhoneNumbers
            .Select(p => p.Number)
            .FirstOrDefault() ?? string.Empty;
    }
}
