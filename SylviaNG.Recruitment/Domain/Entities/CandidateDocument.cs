using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Categorized supporting document uploaded by a candidate (US-006), e.g. NID, education
/// certificate, experience letter. Same shape as JobPostingAttachment.
/// </summary>
public class CandidateDocument : Audit
{
    public long CandidateDocumentId { get; set; }
    public long CandidateProfileId { get; set; }

    public CandidateDocumentTypeEnum DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public bool IsActive { get; set; } = true;

    public CandidateProfile CandidateProfile { get; set; } = null!;
}
