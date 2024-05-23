using System.Text.Json;
using System.Text.Json.Serialization;
using Questrade.Library.PubSubClientHelper.Primitives;

namespace Unit.Providers;

internal sealed class MyDefaultJsonSerializerOptionsProvider : IDefaultJsonSerializerOptionsProvider
{
    public JsonSerializerOptions GetJsonSerializerOptions()
    {
        var settings = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        return settings;
    }
}
