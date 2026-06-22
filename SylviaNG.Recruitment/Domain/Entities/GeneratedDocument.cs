using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class GeneratedDocument : Audit
{
    public long GeneratedDocumentId { get; set; }
    public long DocumentTemplateId { get; set; }
    public long CandidateId { get; set; }
    public long? JobApplicationId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FileFormat { get; set; }
    public DateTime GeneratedAt { get; set; }
    public long GeneratedByUserId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public DocumentTemplate DocumentTemplate { get; set; } = null!;
    public Candidate Candidate { get; set; } = null!;
    public JobApplication? JobApplication { get; set; }
    public User GeneratedByUser { get; set; } = null!;
    public DocumentAcceptance? Acceptance { get; set; }
}
