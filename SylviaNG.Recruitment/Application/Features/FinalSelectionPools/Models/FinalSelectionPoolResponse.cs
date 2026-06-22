namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models
{
    public class FinalSelectionPoolResponse
    {
        public long FinalSelectionPoolId { get; set; }
        public long CandidateId { get; set; }
        public long JobApplicationId { get; set; }
        public DateTime? ExpectedJoiningDate { get; set; }
        public string? JoiningBatch { get; set; }
        public string? OnboardingChecklistJson { get; set; }
        public bool HasJoined { get; set; }
        public DateTime? ActualJoiningDate { get; set; }
        public bool IsActive { get; set; }
    }
}
