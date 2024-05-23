using Domain.Models.Analysis;
using System.Text.Json.Serialization;

namespace Domain.Models;

public class IdentityIntelligencePublisherMessage
{
    [JsonPropertyName("crmUserId")]
    public string? CrmUserId { get; set; }

    [JsonPropertyName("enterpriseProfileId")]
    public string? EnterpriseProfileId { get; set; }

    [JsonPropertyName("profileId")]
    public string? ProfileId { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("addressLine1")]
    public string? AddressLine1 { get; set; }

    [JsonPropertyName("addressLine2")]
    public string? AddressLine2 { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("addressCountryCode")]
    public string? AddressCountryCode { get; set; }

    [JsonPropertyName("nationalId")]
    public string? NationalId { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public string? DateOfBirth { get; set; }

    public static IdentityIntelligencePublisherMessage From(CustomerIdentityAnalysisRequest request, string phoneNumber)
    {
        return new IdentityIntelligencePublisherMessage
        {
            AddressCountryCode = request.AddressCountryCode,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            CrmUserId = request.CrmUserId,
            DateOfBirth = request.DateOfBirth,
            EnterpriseProfileId = request.EnterpriseProfileId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NationalId = request.NationalId,
            Phone = phoneNumber,
            PostalCode = request.PostalCode,
            ProfileId = request.ProfileId,
            State = request.State
        };
    }
}
