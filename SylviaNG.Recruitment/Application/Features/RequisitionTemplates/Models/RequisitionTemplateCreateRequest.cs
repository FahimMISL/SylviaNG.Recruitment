namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models
{
    public class RequisitionTemplateCreateRequest
    {
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string StageConfigJson { get; set; } = string.Empty;
    }
}
