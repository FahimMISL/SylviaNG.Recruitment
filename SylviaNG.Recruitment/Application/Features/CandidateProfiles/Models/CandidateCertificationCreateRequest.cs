using Microsoft.AspNetCore.Http;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    /// <summary>
    /// Multipart form request (scalar fields + an optional certificate file), matching
    /// JobApplicationSubmitRequest's established mixed-content-DTO pattern.
    /// </summary>
    public class CandidateCertificationCreateRequest
    {
        public string CertificationName { get; set; } = string.Empty;
        public string? IssuingOrganization { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CredentialId { get; set; }
        public IFormFile? CertificateFile { get; set; }
    }
}
