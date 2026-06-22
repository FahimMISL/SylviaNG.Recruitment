using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class RequisitionTemplate : Audit
{
    public long RequisitionTemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string StageConfigJson { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
