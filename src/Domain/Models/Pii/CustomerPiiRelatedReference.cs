using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiRelatedReference
{
    [JsonPropertyName("attributeName")]
    public string? AttributeName { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
