using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models
{
    public class CandidateComplaintUpdateRequest
    {
        public string? Category { get; set; }
        public string? Description { get; set; }
        public ComplaintStatusEnum? ComplaintStatus { get; set; }
        public long? AssignedToUserId { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}
