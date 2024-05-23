using Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Config;

[ExcludeFromCodeCoverage]
public class MetricServiceConfig
{
    public CollectLevel CollectLevel { get; set; } = CollectLevel.Basic;
}
