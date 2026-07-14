using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateCertification : Audit
{
    public long CandidateCertificationId { get; set; }
    public long CandidateProfileId { get; set; }

    public string CertificationName { get; set; } = string.Empty;
    public string? IssuingOrganization { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CredentialId { get; set; }
    public string? CertificateFilePath { get; set; }

    public CandidateProfile CandidateProfile { get; set; } = null!;
}
