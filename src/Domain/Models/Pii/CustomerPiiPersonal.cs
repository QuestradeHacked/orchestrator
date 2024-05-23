using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiPersonal
{
    [JsonPropertyName("addresses")]
    public List<CustomerPiiAddress>? Addresses { get; set; } = new();

    [JsonPropertyName("dateOfBirth")]
    public string? DateOfBirth { get; set; }

    [JsonPropertyName("electronicContacts")]
    public List<CustomerPiiElectronicContact> ElectronicContacts { get; set; } = new();

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("lastAnnualKycUpdate")]
    public string? LastAnnualKycUpdate { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("maritalStatus")]
    public string? MaritalStatus { get; set; }

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("phones")]
    public List<CustomerPiiPhone>? Phones { get; set; } = new ();

    [JsonPropertyName("relationships")]
    public List<CustomerPiiRelationship> Relationships { get; set; } = new();

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
