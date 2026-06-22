using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models
{
    public class PreBoardingProfileCreateRequest
    {
        public long CandidateId { get; set; }
        public long JobApplicationId { get; set; }
        public PreBoardingStatusEnum ProfileStatus { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ValidatedAt { get; set; }
        public long? ValidatedByUserId { get; set; }
        public string? CorrectionComments { get; set; }
        public bool IsLocked { get; set; }
    }
}
