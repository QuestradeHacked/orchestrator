using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiDeltaChange
{
    [JsonPropertyName("op")]
    public string? Operation { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    public T? GetValueAsDeltaChangeValues<T>()
    {
        var valueAsString = JsonSerializer.Serialize(Value);
        return JsonSerializer.Deserialize<T>(valueAsString);
    }
}
