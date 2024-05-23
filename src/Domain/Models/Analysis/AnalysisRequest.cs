using MediatR;

namespace Domain.Models.Analysis;

public class AnalysisRequest : IRequest
{
    public virtual string? AccountDetailNumber { get; set; }
    public virtual int? AccountStatusId { get; set; }
    public string? CrmUserId { get; set; }
    public virtual DateTime? EffectiveDate { get; set; }
    public string? EnterpriseProfileId { get; set; }
    public string? ProfileId { get; set; }
}
