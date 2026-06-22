using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models
{
    public class DocumentAcceptanceResponse
    {
        public long DocumentAcceptanceId { get; set; }
        public long GeneratedDocumentId { get; set; }
        public long CandidateId { get; set; }
        public AcceptanceStatusEnum AcceptanceStatus { get; set; }
        public DateTime? ActionDate { get; set; }
        public string? DeclineReason { get; set; }
        public bool IsActive { get; set; }
    }
}
