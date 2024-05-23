using Domain.Constants;
using Domain.Models.Analysis;
using Domain.Models.Pii;

namespace Infra.Subscriber.Extensions.Pii;

public static class CustomerPiiPhoneExtension
{
    private static readonly IList<string> _validOperations = new List<string>{ "add", "replace" };

    public static List<AnalysisRequest>? PhoneUpdateHandler(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        if(!customerPiiUpdateMessage.HasPersonalPhoneNumber()){ return null; }

        return new List<AnalysisRequest>()
        {
            customerPiiUpdateMessage.ToCustomerIdentityAnalysisRequest(),
            customerPiiUpdateMessage.ToCustomerPhoneAnalysisRequest()
        };
    }

    public static bool HasPersonalPhoneNumber(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        return customerPiiUpdateMessage.Customer?.Profile?.Personal?.Phones?.Exists
        (
            p =>p.Status?.ToLower() == "active" &&
            p.PhoneNumberType?.ToLower() == CustomerPiiPhoneNumberType.Personal
        ) ?? false;
    }

    public static bool HasPersonalPhoneChanges(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        var data = customerPiiUpdateMessage;

        return data?.Customer?.Profile?.Personal?.Phones?.Exists
        (
            a =>
            (
                !data.DeltaChanges.Any() &&
                a.PhoneNumberType?.ToLower() == CustomerPiiPhoneNumberType.Personal &&
                a.Status?.ToLower() == "active"
            ) ||
            data.DeltaChanges.Exists
            (
                d => d.Path?.ToLower() == "/profile/personal/phones" &&
                _validOperations.Contains(d.Operation!.ToLower()) &&
                d.GetValueAsDeltaChangeValues<List<CustomerPiiPhone>>()!.Exists
                (
                    v =>v.Status?.ToLower() == "active" &&
                    v.PhoneNumberType?.ToLower() == CustomerPiiPhoneNumberType.Personal
                )
            )
        ) ?? false;
    }

    public static CustomerPiiPhone? GetPersonalPhoneNumber(this CustomerPiiUpdateMessage customerPiiUpdateMessage)
    {
        var phone = customerPiiUpdateMessage.Customer?.Profile?.Personal?.Phones?.Where
        (
            p => p.Status?.ToLower() == "active" &&
            p.PhoneNumberType?.ToLower() == CustomerPiiPhoneNumberType.Personal
        ).FirstOrDefault();

        if
        (
            phone is not null &&
            phone.AreaCode is not null &&
            phone.CountryCode is not null &&
            phone.Exchange is not null &&
            phone.LocalNumber is not null
        )
        {
            phone.PhoneNumber = $"+{phone.CountryCode}{phone.AreaCode}{phone.Exchange}{phone.LocalNumber}";
        }

        return phone;
    }
}
