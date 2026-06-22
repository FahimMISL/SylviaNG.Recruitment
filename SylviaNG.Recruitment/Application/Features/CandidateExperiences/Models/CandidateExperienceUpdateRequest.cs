namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models
{
    public class CandidateExperienceUpdateRequest
    {
        public long? CandidateId { get; set; }
        public string? OrganizationName { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsCurrentJob { get; set; }
        public string? Responsibilities { get; set; }
        public bool? IsActive { get; set; }
    }
}
