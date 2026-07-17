using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A single ordered step within an AssessmentWorkflow (e.g. Written Test, Aptitude Test) (US-052).
/// StageType is a closed enum (unlike PipelineStage.StageType) since the assessment stage types
/// are a fixed, known set rather than admin-defined free text.
/// </summary>
public class AssessmentStage : Audit
{
    public long AssessmentStageId { get; set; }
    public long AssessmentWorkflowId { get; set; }
    public StageTypeEnum StageType { get; set; }
    public int MaxMarks { get; set; }
    public int PassMarks { get; set; }
    public int DurationMinutes { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMandatory { get; set; } = true;

    // Navigation properties
    public AssessmentWorkflow AssessmentWorkflow { get; set; } = null!;
}
