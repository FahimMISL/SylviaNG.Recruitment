namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models
{
    public class RequisitionStageConfigUpdateRequest
    {
        public string? StageName { get; set; }
        public int? StageOrder { get; set; }
        public bool? IsMandatory { get; set; }
        public bool? IsActive { get; set; }
    }
}
