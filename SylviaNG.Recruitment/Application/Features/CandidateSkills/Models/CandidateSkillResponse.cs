namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Models
{
    public class CandidateSkillResponse
    {
        public long CandidateSkillId { get; set; }
        public long CandidateId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? ProficiencyLevel { get; set; }
        public bool IsActive { get; set; }
    }
}
