using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A single ordered step within a HiringPipeline (e.g. "CV Screening", "Technical Interview").
/// StageType is a free-form code rather than an enum so admins can define custom stage types
/// without a code change; PipelineStageTypes carries the built-in suggested set. A stage that
/// happens to be an assessment (written test, aptitude test, etc. - formerly a separate
/// AssessmentWorkflow/AssessmentStage feature, merged in here) just sets MaxMarks/PassMarks;
/// every other stage leaves them null.
/// </summary>
public class PipelineStage : Audit
{
    public long PipelineStageId { get; set; }
    public long HiringPipelineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StageType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? Description { get; set; }
    public string? PassingCriteria { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsMandatory { get; set; } = true;

    // DepartmentId is an external Core-HR reference (no local Department table), same
    // convention as JobPosting.DepartmentId.
    public long? DepartmentId { get; set; }

    public int? EstimatedDurationMinutes { get; set; }
    public int? SlaDays { get; set; }
    public int? MaxMarks { get; set; }
    public int? PassMarks { get; set; }
    public string? ColorBadge { get; set; }
    public string? EmailTemplate { get; set; }
    public bool NotifyCandidateOnEnter { get; set; } = true;
    public bool NotifyInterviewersOnAssign { get; set; } = true;

    // Comma-separated list of required document labels (e.g. "NID,Certificate"). Kept as a
    // simple delimited field rather than a child table since it's descriptive metadata only.
    public string? RequiredDocuments { get; set; }

    public bool AllowCandidateReschedule { get; set; }
    public string? AutoProgressionRule { get; set; }
    public bool ManualApprovalRequired { get; set; }

    // Navigation properties
    public HiringPipeline HiringPipeline { get; set; } = null!;
    public ICollection<PipelineStageInterviewer> Interviewers { get; set; } = new List<PipelineStageInterviewer>();
}
