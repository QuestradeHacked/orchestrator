using System.Text.Json.Serialization;

namespace Domain.Constants;

public static class CustomerPiiPhoneNumberType
{
    [JsonPropertyName("personal")]
    public const string Personal = "primary";

    [JsonPropertyName("work")]
    public const string Work = "telephone daytime";
}
