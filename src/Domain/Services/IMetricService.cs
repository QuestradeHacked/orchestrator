namespace Domain.Services;

public interface IMetricService
{
    void Distribution(string statName, double value, IList<string>? tags = null);

    void Increment(string statName, IList<string> tags);

    IDisposable StartTimer(string statName, IList<string>? tags = null);

    IDisposable TraceTimer(string statName, IList<string>? tags = null);
}
