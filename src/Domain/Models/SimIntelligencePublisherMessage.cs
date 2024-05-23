using Domain.Constants;
using System.Text.Json.Serialization;

namespace Domain.Models;

public class SimIntelligencePublisherMessage
{
    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }

    [JsonPropertyName("accountStatusId")]
    public int? AccountStatusId { get; set; }

    [JsonPropertyName("crmUserId")]
    public string? CrmUserId { get; set; }

    [JsonPropertyName("effectiveDate")]
    public DateTime? EffectiveDate { get; set; }

    [JsonPropertyName("enterpriseProfileId")]
    public string? EnterpriseProfileId { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("profileId")]
    public string? ProfileId { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; } = SourceNames.Source;
}
