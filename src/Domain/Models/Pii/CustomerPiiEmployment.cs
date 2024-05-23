using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiEmployment
{
    [JsonPropertyName("employmentItems")]
    public List<CustomerPiiEmploymentItem> EmploymentItems { get; set; } = new();
}
