using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class AssessmentStage : Audit
{
    public long AssessmentStageId { get; set; }
    public long AssessmentWorkflowId { get; set; }
    public AssessmentTypeEnum AssessmentType { get; set; }
    public string StageName { get; set; } = string.Empty;
    public int StageOrder { get; set; }
    public decimal? PassThreshold { get; set; }
    public decimal? Weight { get; set; }
    public bool IsPassFail { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public AssessmentWorkflow AssessmentWorkflow { get; set; } = null!;
}
