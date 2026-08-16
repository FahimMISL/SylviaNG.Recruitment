namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    // AC1 batch stub: candidates whose OfferLetter.Status == Accepted, the pool HR picks a batch
    // from until the real Final Selection Pool (US-094/EP-12) exists.
    public class JoiningBookletEligibleCandidateResponse
    {
        public long OfferLetterId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateTime OfferJoiningDate { get; set; }
    }
}
