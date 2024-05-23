using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiTax
{
    [JsonPropertyName("canada")]
    public CustomerPiiCanada? Canada { get; set; }

    [JsonPropertyName("crsCertificationAccepted")]
    public bool? CrsCertificationAccepted { get; set; }

    [JsonPropertyName("otherTaxJurisdictions")]
    public List<CustomerPiiTaxJurisdiction> OtherTaxJurisdictions { get; set; } = new();

    [JsonPropertyName("usa")]
    public CustomerPiiUSA? USA { get; set; }
}
