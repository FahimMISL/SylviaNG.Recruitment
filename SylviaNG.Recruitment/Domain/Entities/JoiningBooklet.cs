using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class JoiningBooklet : Audit
{
    public long JoiningBookletId { get; set; }
    public long CandidateId { get; set; }
    public long JobApplicationId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public DateTime GeneratedAt { get; set; }
    public long GeneratedByUserId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
    public User GeneratedByUser { get; set; } = null!;
}
