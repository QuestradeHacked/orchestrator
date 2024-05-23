namespace Domain.Models;

public class PubSubMessage<TData>
{
    public TData? Data { get; set; }

    public string? DataContentType { get; set; }

    public string? Id { get; set; }

    public string? Source { get; set; }

    public string? SpecVersion { get; set; }

    public DateTime? Time { get; set; }

    public string? Type { get; set; }
}
