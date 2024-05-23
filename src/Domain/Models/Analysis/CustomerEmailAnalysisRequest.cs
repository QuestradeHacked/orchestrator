namespace Domain.Models.Analysis;

public class CustomerEmailAnalysisRequest : AnalysisRequest
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? CreatedAt { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? UpdatedAt { get; set; }
    public string ZipCode { get; set; } = string.Empty;
}
