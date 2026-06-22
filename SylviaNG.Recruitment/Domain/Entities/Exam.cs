using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class Exam : Audit
{
    public long ExamId { get; set; }
    public long RequisitionId { get; set; }
    public long? AssessmentStageId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public ExamStatusEnum ExamStatus { get; set; } = ExamStatusEnum.Scheduled;
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public int DurationMinutes { get; set; }
    public decimal TotalMarks { get; set; }
    public decimal? PassMarks { get; set; }
    public string? Instructions { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Requisition Requisition { get; set; } = null!;
    public AssessmentStage? AssessmentStage { get; set; }
    public ICollection<ExamCandidate> ExamCandidates { get; set; } = new List<ExamCandidate>();
}
