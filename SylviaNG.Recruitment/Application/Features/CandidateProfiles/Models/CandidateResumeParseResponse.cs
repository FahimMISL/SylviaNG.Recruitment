using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    /// <summary>
    /// Best-effort result of parsing an uploaded resume (free, local, regex/heuristic-based —
    /// no external AI API). Frontend prefills form controls from this; the candidate must still
    /// review and hit Save per section — nothing here is persisted automatically.
    /// </summary>
    public class CandidateResumeParseResponse
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderEnum? Gender { get; set; }
        public ReligionEnum? Religion { get; set; }
        public MaritalStatusEnum? MaritalStatus { get; set; }
        public List<string> Skills { get; set; } = new();
        public List<CandidateResumeParsedEducation> Educations { get; set; } = new();
        public List<CandidateResumeParsedWorkExperience> WorkExperiences { get; set; } = new();
    }

    public class CandidateResumeParsedEducation
    {
        public string? DegreeTitle { get; set; }
        public string? Institution { get; set; }
        public int? PassingYear { get; set; }
    }

    public class CandidateResumeParsedWorkExperience
    {
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
    }
}
