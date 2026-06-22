namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models
{
    public class CandidateComplaintCreateRequest
    {
        public long CandidateId { get; set; }
        public long? JobApplicationId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long? AssignedToUserId { get; set; }
    }
}
