using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models
{
    public class CandidateComplaintResponse
    {
        public long CandidateComplaintId { get; set; }
        public long CandidateId { get; set; }
        public long? JobApplicationId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ComplaintStatusEnum ComplaintStatus { get; set; }
        public long? AssignedToUserId { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
