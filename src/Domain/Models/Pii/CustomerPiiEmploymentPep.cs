using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiEmploymentPep
{
    [JsonPropertyName("isPoliticallyExposed")]
    public bool? IsPoliticallyExposed { get; set; }

    [JsonPropertyName("politicallyExposedPersons")]
    public List<CustomerPiiPoliticallyExposedPerson> PoliticallyExposedPersons { get; set; } = new();
}
