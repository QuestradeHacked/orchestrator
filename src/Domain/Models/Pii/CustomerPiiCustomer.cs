using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiCustomer
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("isDeactivated")]
    public bool? IsDeactivated { get; set; }

    [JsonPropertyName("isIdentityVerified")]
    public bool? IsIdentityVerified { get; set; }

    [JsonPropertyName("isQuestradeEmployee")]
    public bool? IsQuestradeEmployee { get; set; }

    [JsonPropertyName("masterProfileId")]
    public string? MasterProfileId { get; set; }

    [JsonPropertyName("profile")]
    public CustomerPiiProfile? Profile { get; set; }

    [JsonPropertyName("relatedReferences")]
    public List<CustomerPiiRelatedReference> RelatedReferences { get; set; } = new();

    [JsonPropertyName("revision")]
    public string? Revision { get; set; }
}
