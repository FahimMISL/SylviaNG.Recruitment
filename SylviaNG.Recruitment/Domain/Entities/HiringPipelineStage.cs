using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class HiringPipelineStage : Audit
{
    public long HiringPipelineStageId { get; set; }
    public long JobPostingId { get; set; }
    public string StageName { get; set; } = string.Empty;
    public string StageType { get; set; } = string.Empty;
    public int StageOrder { get; set; }
    public bool IsMandatory { get; set; } = true;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public JobPosting JobPosting { get; set; } = null!;
}
