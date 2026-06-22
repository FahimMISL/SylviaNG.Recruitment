using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ExamCandidate : Audit
{
    public long ExamCandidateId { get; set; }
    public long ExamId { get; set; }
    public long JobApplicationId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public decimal? ObtainedMarks { get; set; }
    public bool IsPassed { get; set; }
    public bool IsAutoGraded { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Exam Exam { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
    public ICollection<ExamAnswer> Answers { get; set; } = new List<ExamAnswer>();
}
