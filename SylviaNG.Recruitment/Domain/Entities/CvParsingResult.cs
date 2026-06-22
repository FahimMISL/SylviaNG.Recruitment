using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CvParsingResult : Audit
{
    public long CvParsingResultId { get; set; }
    public long CandidateId { get; set; }
    public long CandidateDocumentId { get; set; }
    public string? ParsedDataJson { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public bool IsReviewedByCandidate { get; set; }
    public string? ParsingErrors { get; set; }
    public DateTime ParsedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public CandidateDocument CandidateDocument { get; set; } = null!;
}
