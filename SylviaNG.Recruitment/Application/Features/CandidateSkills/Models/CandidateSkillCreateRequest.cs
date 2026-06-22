namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Models
{
    public class CandidateSkillCreateRequest
    {
        public long CandidateId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? ProficiencyLevel { get; set; }
    }
}
