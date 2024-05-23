using Microsoft.Extensions.Logging;

namespace Infra.Util;

public class CorrelationContext
{
    public string CorrelationId { get; private set; }

    public CorrelationContext()
    {
        CorrelationId = Guid.NewGuid().ToString();
    }

    public void AddCorrelationIdScope(ILogger logger, string? correlationId)
    {
        if (correlationId is not null)
        {
            CorrelationId = correlationId;
        }

        LogCorrelationId(logger, CorrelationId);
    }

    private static readonly Func<ILogger, string, IDisposable> LogCorrelationId =
        LoggerMessage.DefineScope<string>("{CorrelationId}");
}
