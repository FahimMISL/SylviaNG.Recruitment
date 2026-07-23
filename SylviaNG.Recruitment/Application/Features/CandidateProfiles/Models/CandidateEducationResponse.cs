using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateEducationResponse
    {
        public long CandidateEducationId { get; set; }
        public long DegreeId { get; set; }
        public long? EducationBoardId { get; set; }
        public string Institution { get; set; } = string.Empty;
        public long? UniversityLibraryItemId { get; set; }
        public EducationLevelEnum? EducationLevel { get; set; }
        public int PassingYear { get; set; }
        public GradingSystemEnum? GradingSystem { get; set; }
        public string Result { get; set; } = string.Empty;
        public string? MajorSubject { get; set; }
    }
}
