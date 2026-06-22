namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models
{
    public class RequisitionStageConfigResponse
    {
        public long RequisitionStageConfigId { get; set; }
        public long RequisitionId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public int StageOrder { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
    }
}
