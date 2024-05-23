using Domain.Models;

namespace Infra.Subscriber.Extensions;

public static class DomesticAddressExtension
{
    public static bool HasDomesticAddressChanges(this CustomerProfileUpdatedMessage data)
    {
        var domesticChanged = data.DomesticAddress;
        return domesticChanged != null
               && EnsureAllRequiredFieldsAreFilledIn(domesticChanged);
    }

    public static IEnumerable<IRequestOrchestrator> HandleDomesticAddressChanges(this CustomerProfileUpdatedMessage data)
    {
        return data.HasDomesticAddressChanges()
            ? MakeRequestOrchestrators(data)
            : Enumerable.Empty<IRequestOrchestrator>();
    }

    private static bool EnsureAllRequiredFieldsAreFilledIn(CustomerDomesticAddress domesticAddress)
    {
        return !string.IsNullOrEmpty(domesticAddress.StreetNumber)
               && !string.IsNullOrEmpty(domesticAddress.StreetName)
               && !string.IsNullOrEmpty(domesticAddress.City)
               && !string.IsNullOrEmpty(domesticAddress.Province)
               && !string.IsNullOrEmpty(domesticAddress.PostalCode);
    }

    private static IEnumerable<IRequestOrchestrator> MakeRequestOrchestrators(
        CustomerProfileUpdatedMessage customerProfileUpdatedMessage)
    {
        var emailIntelligenceRequest = new CustomerProfileEmailUpdatedRequest
        {
            CrmUserId = customerProfileUpdatedMessage.UserId,
            ProfileId = customerProfileUpdatedMessage.PersonId
        };

        var simIntelligenceRequest = new CustomerProfilePhoneUpdatedRequest
        {
            CrmUserId = customerProfileUpdatedMessage.UserId,
            ProfileId = customerProfileUpdatedMessage.PersonId
        };

        var identityIntelligenceRequest = new CustomerProfilePhoneUpdatedRequest
        {
            CrmUserId = customerProfileUpdatedMessage.UserId,
            ProfileId = customerProfileUpdatedMessage.PersonId
        };

        return new List<IRequestOrchestrator> { emailIntelligenceRequest, simIntelligenceRequest, identityIntelligenceRequest };
    }
}
