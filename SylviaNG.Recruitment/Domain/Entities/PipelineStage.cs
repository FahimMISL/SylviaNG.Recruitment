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
public class PipelineStage : Audit, ICompanyScoped
{
    public long PipelineStageId { get; set; }

    // Multi-tenant: mirrors the parent HiringPipeline's CompanyId, stamped at creation.
    public long? CompanyId { get; set; }
    public long HiringPipelineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StageType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? Description { get; set; }
    public string? PassingCriteria { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsMandatory { get; set; } = true;

    public int? EstimatedDurationMinutes { get; set; }
    public int? SlaDays { get; set; }
    public int? MaxMarks { get; set; }
    public int? PassMarks { get; set; }

    // Comma-separated list of required document labels (e.g. "NID,Certificate"). Kept as a
    // simple delimited field rather than a child table since it's descriptive metadata only.
    public string? RequiredDocuments { get; set; }

    public string? AutoProgressionRule { get; set; }

    // The stage to auto-advance a candidate into once their Score on THIS stage meets
    // PassMarks. Stored as the target's DisplayOrder, not its PipelineStageId - stage IDs are
    // cleared and re-inserted on every pipeline edit (see JobApplicationStageProgressConfiguration's
    // comment on the same tradeoff), but DisplayOrder is exactly what the edit form already
    // has in hand for every stage, before or after a save. Null = no auto-progression configured.
    public int? AutoProgressionTargetDisplayOrder { get; set; }
    public bool ManualApprovalRequired { get; set; }

    // Navigation properties
    public HiringPipeline HiringPipeline { get; set; } = null!;
}
