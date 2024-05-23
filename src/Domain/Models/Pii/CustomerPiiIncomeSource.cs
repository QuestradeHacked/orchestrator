using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiIncomeSource
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
