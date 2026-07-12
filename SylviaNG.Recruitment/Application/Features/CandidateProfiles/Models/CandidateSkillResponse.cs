namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateSkillResponse
    {
        public long CandidateSkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public long? SkillLibraryItemId { get; set; }
        public string? ProficiencyLevel { get; set; }
    }
}
