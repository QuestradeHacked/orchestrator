using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiElectronicContact
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("isVerified")]
    public bool? IsVerified { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("subtype")]
    public string? Subtype { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
