namespace Domain.Models.Analysis;

public class CustomerIdentityAnalysisRequest : AnalysisRequest
{
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? AddressCountryCode { get; set; }
    public string? City { get; set; }
    public string? DateOfBirth { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalId { get; set; }
    public IEnumerable<string> PhoneNumbers { get; set; } = Enumerable.Empty<string>();
    public string? PostalCode { get; set; }
    public string? State { get; set; }
}
