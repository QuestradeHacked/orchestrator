using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiCanada
{
    [JsonPropertyName("consentToUseSIN")]
    public bool? ConsentToUseSIN { get; set; }

    [JsonPropertyName("isResident")]
    public bool? IsResident { get; set; }

    [JsonPropertyName("isTaxResident")]
    public bool? IsTaxResident { get; set; }

    [JsonPropertyName("sin")]
    public string? SIN { get; set; }

    [JsonPropertyName("sinExpiryDate")]
    public string? SINExpiryDate { get; set; }
}
