using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiUpdateMessage
{
    [JsonPropertyName("customer")]
    public CustomerPiiCustomer? Customer { get; set; }

    [JsonPropertyName("deltaChanges")]
    public List<CustomerPiiDeltaChange> DeltaChanges { get; set; } = new();

}
