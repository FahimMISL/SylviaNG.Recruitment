using Microsoft.AspNetCore.Http;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>
    /// Anonymous/authenticated candidate apply request submitted via the career portal or the
    /// internal job board (multipart form: scalar fields + a CV file). The audience/source
    /// (External vs Internal) is not part of this DTO - it is determined by which controller/
    /// route the request came in through and passed alongside the request (see
    /// JobApplicationSubmitCommand.Source).
    /// </summary>
    public class JobApplicationSubmitRequest
    {
        public long JobPostingId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public string? CandidatePhone { get; set; }
        public string? CandidateNationalId { get; set; }
        public string? CoverLetter { get; set; }
        public IFormFile? Resume { get; set; }

        // EP-17/US-127: optional, candidate-declared at apply time - feeds fee-waiver rule
        // matching and F1 reconciliation reporting (JobApplicationService.SubmitAsync).
        public long? SpecialCategoryId { get; set; }
        public long? ReferralSourceId { get; set; }

        // Proof of the claimed SpecialCategoryId - a matching WaiverRule only waives the fee
        // when this is attached, otherwise the applicant still pays (JobApplicationService.SubmitAsync).
        public IFormFile? WaiverProofDocument { get; set; }
    }
}
