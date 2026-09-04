namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Models
{
    // Warn-only signal for the "one candidate, two concurrent Hired applications" gap - nothing
    // in the pipeline/offer/onboarding flow is scoped to the candidate as a whole (everything
    // keys off JobApplicationId), so a person can be Hired for two postings at once with no
    // technical block. This just surfaces the fact to HR at offer-generation time; it never blocks.
    public class CandidateHireConflictResponse
    {
        public long JobApplicationId { get; set; }
        public string JobPostingTitle { get; set; } = string.Empty;
        public string ApplicationStatus { get; set; } = string.Empty;
    }
}
