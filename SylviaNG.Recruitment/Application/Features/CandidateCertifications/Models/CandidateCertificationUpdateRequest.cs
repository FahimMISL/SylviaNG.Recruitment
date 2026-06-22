namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models
{
    public class CandidateCertificationUpdateRequest
    {
        public long? CandidateId { get; set; }
        public string? CertificationName { get; set; }
        public string? IssuingOrganization { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CredentialId { get; set; }
        public string? CredentialUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
