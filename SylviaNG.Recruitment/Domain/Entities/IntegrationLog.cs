using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class IntegrationLog : Audit
{
    public long IntegrationLogId { get; set; }
    public IntegrationTypeEnum IntegrationType { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public string? RequestPayloadJson { get; set; }
    public string? ResponsePayloadJson { get; set; }
    public int? HttpStatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ExecutedAt { get; set; }
    public long? DurationMs { get; set; }
    public bool IsActive { get; set; } = true;
}
