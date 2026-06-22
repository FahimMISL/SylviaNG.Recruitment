using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models
{
    public class IntegrationLogResponse
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
        public bool IsActive { get; set; }
    }
}
