namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models
{
    public class RequisitionStageConfigCreateRequest
    {
        public long RequisitionId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public int StageOrder { get; set; }
        public bool IsMandatory { get; set; } = true;
    }
}
