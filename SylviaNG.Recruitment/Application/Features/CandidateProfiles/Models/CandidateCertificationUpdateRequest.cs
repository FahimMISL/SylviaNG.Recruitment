using Microsoft.AspNetCore.Http;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateCertificationUpdateRequest
    {
        public string CertificationName { get; set; } = string.Empty;
        public string? IssuingOrganization { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CredentialId { get; set; }

        /// <summary>
        /// If provided, replaces the existing certificate file (old one is deleted).
        /// If null, the existing file (if any) is left untouched.
        /// </summary>
        public IFormFile? CertificateFile { get; set; }
    }
}
