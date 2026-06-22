namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models
{
    public class RequisitionTemplateUpdateRequest
    {
        public string? TemplateName { get; set; }
        public string? Description { get; set; }
        public string? StageConfigJson { get; set; }
        public bool? IsActive { get; set; }
    }
}
