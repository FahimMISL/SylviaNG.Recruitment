using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class RequisitionStageConfig : Audit
{
    public long RequisitionStageConfigId { get; set; }
    public long RequisitionId { get; set; }
    public string StageName { get; set; } = string.Empty;
    public int StageOrder { get; set; }
    public bool IsMandatory { get; set; } = true;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Requisition Requisition { get; set; } = null!;
}
