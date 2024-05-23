using Domain.Exceptions;
using Domain.Models.Analysis;
using Domain.Models.Pii;

namespace Infra.Subscriber.Extensions.Pii;

public static class CustomerPiiUpdateMessageExtension
{
    public static CustomerPhoneAnalysisRequest ToCustomerPhoneAnalysisRequest
    (
        this CustomerPiiUpdateMessage customerPiiUpdateMessage
    )
    {
        try
        {
            var data = customerPiiUpdateMessage;
            var phoneNumber = customerPiiUpdateMessage.GetPersonalPhoneNumber()!;

            return new CustomerPhoneAnalysisRequest
            {
                CrmUserId = customerPiiUpdateMessage.GetCrmUserId(),
                EnterpriseProfileId = data.Customer?.MasterProfileId,
                PhoneNumbers = new List<string>{ phoneNumber.PhoneNumber! },
                ProfileId = data.Customer!.Id
            };
        }
        catch(Exception)
        {
            throw new ConvertToAnalysisRequestException();
        }
    }

    public static CustomerEmailAnalysisRequest ToCustomerEmailAnalysisRequest
    (
        this CustomerPiiUpdateMessage customerPiiUpdateMessage
    )
    {
        try
        {
            var data = customerPiiUpdateMessage;
            var electronicContact = customerPiiUpdateMessage.GetPrimaryEmail()!;
            var phoneNumber = customerPiiUpdateMessage.GetPersonalPhoneNumber();
            var primaryAddress = customerPiiUpdateMessage.GetPrimaryAddress()!;

            return new CustomerEmailAnalysisRequest
            {
                City = primaryAddress?.City ?? string.Empty,
                Country = primaryAddress?.Country ?? string.Empty,
                CrmUserId = customerPiiUpdateMessage.GetCrmUserId(),
                Email = electronicContact.Value!,
                EnterpriseProfileId = data.Customer?.MasterProfileId,
                FirstName = data.Customer!.Profile!.Personal!.FirstName!,
                LastName = data.Customer!.Profile!.Personal!.LastName!,
                PhoneNumber = phoneNumber?.PhoneNumber ?? string.Empty,
                ProfileId = data.Customer!.Id,
                State = primaryAddress?.Province ?? string.Empty,
                Street = primaryAddress?.StreetName ?? string.Empty,
                UpdatedAt = electronicContact.UpdatedAt,
                ZipCode = primaryAddress?.PostalCode ?? string.Empty
            };
        }
        catch(Exception)
        {
            throw new ConvertToAnalysisRequestException();
        }
    }

    public static CustomerIdentityAnalysisRequest ToCustomerIdentityAnalysisRequest
    (
        this CustomerPiiUpdateMessage customerPiiUpdateMessage
    )
    {
        try
        {
            var address = customerPiiUpdateMessage.GetPrimaryAddress()!;
            var data = customerPiiUpdateMessage;
            var phoneNumber = customerPiiUpdateMessage.GetPersonalPhoneNumber();
            var phoneNumbers = new List<string>();

            if(phoneNumber is not null){ phoneNumbers.Add(phoneNumber.PhoneNumber!); }

            return new CustomerIdentityAnalysisRequest
            {
                AddressLine1 = address is not null ? $"{address.StreetNumber} {address.StreetName}" : string.Empty,
                AddressCountryCode =address?.CountryCode ?? string.Empty,
                City = address?.City ?? string.Empty,
                CrmUserId = customerPiiUpdateMessage.GetCrmUserId(),
                DateOfBirth = data.Customer!.Profile!.Personal!.DateOfBirth,
                EnterpriseProfileId = data.Customer!.MasterProfileId,
                FirstName = data.Customer.Profile.Personal.FirstName,
                LastName = data.Customer.Profile.Personal.LastName,
                PhoneNumbers = phoneNumbers,
                PostalCode = address?.PostalCode ?? string.Empty,
                ProfileId = data.Customer!.Id,
                State = address?.Province ?? string.Empty
            };
        }
        catch(Exception)
        {
            throw new ConvertToAnalysisRequestException();
        }
    }
}
