using System.Text.Json.Serialization;

namespace Domain.Models.Pii;

public class CustomerPiiEmploymentItem
{
    [JsonPropertyName("employerAddress")]
    public CustomerPiiAddress? EmployerAddress { get; set; }

    [JsonPropertyName("employerName")]
    public string? EmployerName { get; set; }

    [JsonPropertyName("employmentAffiliation")]
    public CustomerPiiEmploymentAffiliation? EmploymentAffiliation { get; set; }

    [JsonPropertyName("employmentId")]
    public string? EmploymentId { get; set; }

    [JsonPropertyName("employmentInsider")]
    public CustomerPiiEmploymentInsider? EmploymentInsider { get; set; }

    [JsonPropertyName("employmentPep")]
    public CustomerPiiEmploymentPep? EmploymentPep { get; set; }

    [JsonPropertyName("employmentSpouseAffiliation")]
    public CustomerPiiEmploymentAffiliation? EmploymentSpouseAffiliation { get; set; }

    [JsonPropertyName("employmentSubType")]
    public string? EmploymentSubType { get; set; }

    [JsonPropertyName("employmentType")]
    public string? EmploymentType { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("incomeSource")]
    public CustomerPiiIncomeSource? IncomeSource { get; set; }

    [JsonPropertyName("typeOfBusiness")]
    public string? TypeOfBusiness { get; set; }

    [JsonPropertyName("jobTitle")]
    public string? JobTitle { get; set; }

    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("workPhoneNumber")]
    public string? WorkPhoneNumber { get; set; }
}
