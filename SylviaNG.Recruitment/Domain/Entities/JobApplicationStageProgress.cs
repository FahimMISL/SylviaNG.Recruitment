using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A candidate's progress through one stage of the hiring pipeline their job posting was
/// assigned at apply time (US-042). PipelineStageId is a soft reference, not an FK -
/// HiringPipelineService.UpdateAsync clears and re-inserts PipelineStage rows on every edit,
/// so a hard FK would either cascade-delete in-flight candidate progress or block pipeline
/// edits entirely. StageName/StageType/DisplayOrder are snapshotted at first-provision time
/// so a candidate's tracker stays stable even if the pipeline template changes later.
/// </summary>
public class JobApplicationStageProgress : Audit
{
    public long JobApplicationStageProgressId { get; set; }
    public long JobApplicationId { get; set; }
    public long PipelineStageId { get; set; }
    public string StageName { get; set; } = string.Empty;
    public string StageType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public new StageProgressStatusEnum Status { get; set; } = StageProgressStatusEnum.Pending;
    public DateTime? ScheduledDate { get; set; }
    public string? MeetingLink { get; set; }
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? LastUpdatedByUserName { get; set; }

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
}
