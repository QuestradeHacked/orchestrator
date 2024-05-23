using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiPoliticallyExposedPerson
{
    [JsonPropertyName("isPoliticallyExposed")]
    public bool? IsPoliticallyExposed { get; set; }

    [JsonPropertyName("politicallyExposedId")]
    public string? PoliticallyExposedId { get; set; }

    [JsonPropertyName("politicallyExposedName")]
    public string? PoliticallyExposedName { get; set; }

    [JsonPropertyName("politicallyExposedReason")]
    public string? PoliticallyExposedReason { get; set; }

    [JsonPropertyName("politicallyExposedSourceOfFunds")]
    public string? PoliticallyExposedSourceOfFunds { get; set; }

    [JsonPropertyName("politicallyExposedSourceOfWealth")]
    public string? PoliticallyExposedSourceOfWealth { get; set; }
}
