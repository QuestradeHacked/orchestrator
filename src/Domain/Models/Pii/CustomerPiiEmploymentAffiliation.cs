using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiEmploymentAffiliation
{
    [JsonPropertyName("isAffiliated")]
    public bool? IsAffiliated { get; set; }
}
