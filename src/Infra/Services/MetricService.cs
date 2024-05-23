using Domain.Models;
using Domain.Services;
using Infra.Config;
using StatsdClient;

namespace Infra.Services;

public class MetricService : IMetricService
{
    private readonly IDogStatsd _dogStatsd;

    private readonly MetricServiceConfig? _metricServiceConfig = default;

    private class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public MetricService(IDogStatsd dogStatsd)
    {
        _dogStatsd = dogStatsd;
    }

    public void Distribution(string statName, double value, IList<string>? tags = null)
    {
        tags ??= new List<string>();

        AddEnvironment(tags);

        _dogStatsd.Distribution(statName, value, tags: ToArray(tags));
    }

    public void Increment(string statName, IList<string> tags)
    {
        AddEnvironment(tags);

        _dogStatsd.Increment(statName, tags: ToArray(tags));
    }

    public IDisposable StartTimer(string statName, IList<string>? tags = null)
    {
        tags ??= new List<string>();

        AddEnvironment(tags);

        return _dogStatsd.StartTimer(statName, tags: ToArray(tags));
    }

    public IDisposable TraceTimer(string statName, IList<string>? tags = null)
    {
        return _metricServiceConfig?.CollectLevel == CollectLevel.Debug ? StartTimer(statName, tags) : new EmptyDisposable();
    }

    private static void AddEnvironment(ICollection<string> tags)
    {
        tags.Add($"env:{Util.Environment.Name.ToUpperInvariant()}");
    }

    private static string[] ToArray(ICollection<string> collection)
    {
        var array = new string[collection.Count];

        collection.CopyTo(array, 0);

        return array;
    }
}
