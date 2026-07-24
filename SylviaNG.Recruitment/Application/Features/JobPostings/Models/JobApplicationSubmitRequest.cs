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
    }
}
