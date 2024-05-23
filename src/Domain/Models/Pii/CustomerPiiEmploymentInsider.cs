using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiEmploymentInsider
{
    [JsonPropertyName("isInsider")]
    public bool? IsInsider { get; set; }
}
