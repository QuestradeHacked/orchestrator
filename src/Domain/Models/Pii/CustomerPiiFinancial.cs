using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiFinancial
{
    [JsonPropertyName("annualIncome")]
    public decimal? AnnualIncome { get; set; }

    [JsonPropertyName("assets")]
    public decimal? Assets { get; set; }

    [JsonPropertyName("liabilities")]
    public decimal? Liabilities { get; set; }

    [JsonPropertyName("liquidAssets")]
    public decimal? LiquidAssets { get; set; }

    [JsonPropertyName("netWorth")]
    public decimal? NetWorth { get; set; }
}
