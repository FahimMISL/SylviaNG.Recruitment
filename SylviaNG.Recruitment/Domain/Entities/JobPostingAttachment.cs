using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Represents a supporting document attached to a job posting (e.g. circular PDF).
/// </summary>
public class JobPostingAttachment : Audit
{
    public long JobPostingAttachmentId { get; set; }
    public long JobPostingId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public JobPosting JobPosting { get; set; } = null!;
}
