using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    public class AssessmentStageResponse
    {
        public long AssessmentStageId { get; set; }
        public StageTypeEnum StageType { get; set; }
        public int MaxMarks { get; set; }
        public int PassMarks { get; set; }
        public int DurationMinutes { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMandatory { get; set; }
    }
}
