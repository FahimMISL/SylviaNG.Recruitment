using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateEducationCreateRequest
    {
        public long DegreeId { get; set; }
        public long? EducationBoardId { get; set; }
        public string Institution { get; set; } = string.Empty;
        public long? UniversityLibraryItemId { get; set; }
        public EducationLevelEnum? EducationLevel { get; set; }
        public int PassingYear { get; set; }
        public GradingSystemEnum? GradingSystem { get; set; }
        public string Result { get; set; } = string.Empty;
        public long? MajorSubjectSscHscId { get; set; }
        public long? MajorSubjectUniversityId { get; set; }
        public string? MajorSubjectOtherText { get; set; }
    }
}
