namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models
{
    public class FinalSelectionPoolResponse
    {
        public long FinalSelectionPoolId { get; set; }
        public long OfferLetterId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string? BatchLabel { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool HasJoined { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime EnteredPoolAt { get; set; }

        /// <summary>Null when the candidate hasn't touched the pre-boarding form yet (no
        /// PreBoardingSubmission row exists), otherwise "Draft" or "Submitted".</summary>
        public string? PreBoardingStatus { get; set; }
    }
}
