using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiUSA
{
    [JsonPropertyName("isBornInUS")]
    public bool? IsBornInUs { get; set; }

    [JsonPropertyName("isPersonUS")]
    public bool? IsPersonUs { get; set; }

    [JsonPropertyName("isRenouncedCitizenship")]
    public bool? IsRenouncedCitizenship { get; set; }

    [JsonPropertyName("ssn")]
    public string? Ssn { get; set; }
}
