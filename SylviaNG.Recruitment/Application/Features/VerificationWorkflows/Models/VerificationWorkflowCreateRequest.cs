using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models
{
    public class VerificationWorkflowCreateRequest
    {
        public long CandidateId { get; set; }
        public long JobApplicationId { get; set; }
        public VerificationStatusEnum OverallStatus { get; set; }
        public long InitiatedByUserId { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
