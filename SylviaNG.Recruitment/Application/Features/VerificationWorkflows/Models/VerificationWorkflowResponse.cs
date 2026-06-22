using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models
{
    public class VerificationWorkflowResponse
    {
        public long VerificationWorkflowId { get; set; }
        public long CandidateId { get; set; }
        public long JobApplicationId { get; set; }
        public VerificationStatusEnum OverallStatus { get; set; }
        public long InitiatedByUserId { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
