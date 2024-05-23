using System.Text.Json.Serialization;
using Google.Api;

namespace Domain.Models.Pii;

public class CustomerPiiPhone
{
    [JsonPropertyName("areaCode")]
    public string? AreaCode { get; set;}

    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("isVerified")]
    public bool? IsVerified { get; set; }

    [JsonPropertyName("localNumber")]
    public string? LocalNumber{ get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("phoneNumberType")]
    public string? PhoneNumberType { get; set; }

    [JsonPropertyName("reasonForInternationalPhone")]
    public string? ReasonForInternationalPhone { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }
}
