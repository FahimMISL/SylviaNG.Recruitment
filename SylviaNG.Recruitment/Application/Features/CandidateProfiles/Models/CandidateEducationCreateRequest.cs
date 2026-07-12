using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateEducationCreateRequest
    {
        public string DegreeTitle { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public EducationLevelEnum? EducationLevel { get; set; }
        public int PassingYear { get; set; }
        public string Result { get; set; } = string.Empty;
        public string? MajorSubject { get; set; }
    }
}
