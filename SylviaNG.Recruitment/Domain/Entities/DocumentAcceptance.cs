using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class DocumentAcceptance : Audit
{
    public long DocumentAcceptanceId { get; set; }
    public long GeneratedDocumentId { get; set; }
    public long CandidateId { get; set; }
    public AcceptanceStatusEnum AcceptanceStatus { get; set; } = AcceptanceStatusEnum.Pending;
    public DateTime? ActionDate { get; set; }
    public string? DeclineReason { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public GeneratedDocument GeneratedDocument { get; set; } = null!;
    public Candidate Candidate { get; set; } = null!;
}
