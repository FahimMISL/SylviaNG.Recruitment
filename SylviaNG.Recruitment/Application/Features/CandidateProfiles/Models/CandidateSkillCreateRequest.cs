namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateSkillCreateRequest
    {
        public string SkillName { get; set; } = string.Empty;
        public long? SkillLibraryItemId { get; set; }
        public string? ProficiencyLevel { get; set; }
    }
}
