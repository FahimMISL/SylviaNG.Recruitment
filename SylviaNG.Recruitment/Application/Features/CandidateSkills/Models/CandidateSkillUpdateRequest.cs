namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Models
{
    public class CandidateSkillUpdateRequest
    {
        public string? SkillName { get; set; }
        public string? ProficiencyLevel { get; set; }
        public bool? IsActive { get; set; }
    }
}
