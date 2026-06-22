using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateEducation : Audit
{
    public long CandidateEducationId { get; set; }
    public long CandidateId { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string? FieldOfStudy { get; set; }
    public string? Institution { get; set; }
    public string? Board { get; set; }
    public int? PassingYear { get; set; }
    public string? Result { get; set; }
    public string? ResultScale { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
}
