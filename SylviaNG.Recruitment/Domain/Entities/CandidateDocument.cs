using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateDocument : Audit
{
    public long CandidateDocumentId { get; set; }
    public long CandidateId { get; set; }
    public CandidateDocumentTypeEnum DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? FileSizeBytes { get; set; }
    public byte[]? FileData { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
}
