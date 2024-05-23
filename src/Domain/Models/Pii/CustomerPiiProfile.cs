using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiProfile
{
    [JsonPropertyName("employment")]
    public CustomerPiiEmployment? Employment { get; set; }

    [JsonPropertyName("financial")]
    public CustomerPiiFinancial? Financial { get; set; }

    [JsonPropertyName("personal")]
    public CustomerPiiPersonal? Personal { get; set; }

    [JsonPropertyName("tax")]
    public CustomerPiiTax? Tax { get; set; }
}
