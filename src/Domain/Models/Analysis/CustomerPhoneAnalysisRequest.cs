namespace Domain.Models.Analysis;

public class CustomerPhoneAnalysisRequest : AnalysisRequest
{
    public IEnumerable<string> PhoneNumbers { get; set; } = Enumerable.Empty<string>();
}
