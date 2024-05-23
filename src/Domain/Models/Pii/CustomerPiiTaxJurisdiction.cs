using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiTaxJurisdiction
{
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("tin")]
    public string? Tin { get; set; }

    [JsonPropertyName("tinAbsentReason")]
    public string? TinAbsentReason { get; set; }

    [JsonPropertyName("tinAbsentReasonCategory")]
    public string? TinAbsentReasonCategory { get; set; }

}
