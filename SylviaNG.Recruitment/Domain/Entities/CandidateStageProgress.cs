using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateStageProgress : Audit
{
    public long CandidateStageProgressId { get; set; }
    public long JobApplicationId { get; set; }
    public long HiringPipelineStageId { get; set; }
    public new string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public string? MeetingLink { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public JobApplication JobApplication { get; set; } = null!;
    public HiringPipelineStage HiringPipelineStage { get; set; } = null!;
}
