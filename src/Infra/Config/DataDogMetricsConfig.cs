using System.Diagnostics.CodeAnalysis;

namespace Infra.Config;

[ExcludeFromCodeCoverage]
public class DataDogMetricsConfig
{
    public string HostName { get; set; } = default!;

    public string Prefix { get; set; } = default!;
}
