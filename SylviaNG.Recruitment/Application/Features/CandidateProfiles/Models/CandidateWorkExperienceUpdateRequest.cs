namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateWorkExperienceUpdateRequest
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string Responsibilities { get; set; } = string.Empty;
        public string? Location { get; set; }
    }
}
