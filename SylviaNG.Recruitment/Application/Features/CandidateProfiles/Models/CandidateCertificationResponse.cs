namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateCertificationResponse
    {
        public long CandidateCertificationId { get; set; }
        public string CertificationName { get; set; } = string.Empty;
        public string? IssuingOrganization { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CredentialId { get; set; }
        public string? CertificateFilePath { get; set; }
    }
}
