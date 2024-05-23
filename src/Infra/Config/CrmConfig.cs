using System.Diagnostics.CodeAnalysis;

namespace Infra.Config;

[ExcludeFromCodeCoverage]
public class CrmConfig
{
    public string Endpoint { get; set; } = default!;

    public string Token { get; set; } = default!;
}
