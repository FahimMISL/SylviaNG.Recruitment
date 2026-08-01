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

    // Score on this stage (CV screening rating, exam mark, interview weighted score - whatever
    // the stage type calls for). HR enters it manually here regardless of stage type; feeds
    // PipelineStage.AutoProgressionTargetDisplayOrder when set alongside a Completed status.
    public decimal? Score { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? LastUpdatedByUserName { get; set; }

    // EP-14 US-105/US-109: snapshotted alongside StageName/StageType/DisplayOrder for the same
    // reason - PipelineStageId is a soft reference and HiringPipelineService.UpdateAsync
    // clears/re-inserts PipelineStage rows on every pipeline edit, so a live join to the
    // current PipelineStage would silently go stale for in-flight candidates.
    public bool RequiresManualApproval { get; set; }
    public int? SlaDaysSnapshot { get; set; }

    // Stamped on the Pending/other -> InProgress transition (PipelineProgressMapper.ApplyUpdate /
    // JobApplicationStageProgressService.BulkAdvanceToStageAsync), mirroring how CompletedAt is
    // stamped on -> Completed. Backs "Days in Current Stage" (US-109 AC1/AC2).
    public DateTime? StageEnteredAt { get; set; }

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
}
