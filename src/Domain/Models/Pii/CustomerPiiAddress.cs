using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiAddress
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("formattedAddress")]
    public List<string> FormattedAddress { get; set; } = new();

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("province")]
    public string? Province { get; set; }

    [JsonPropertyName("provinceCode")]
    public string? ProvinceCode { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("streetName")]
    public string? StreetName { get; set; }

    [JsonPropertyName("streetType")]
    public string? StreetType { get; set; }

    [JsonPropertyName("streetNumber")]
    public string? StreetNumber { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("unitNumber")]
    public string? UnitNumber { get; set; }
}
