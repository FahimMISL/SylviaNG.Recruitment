using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    public class AssessmentStageRequest
    {
        public StageTypeEnum StageType { get; set; }
        public int MaxMarks { get; set; }
        public int PassMarks { get; set; }
        public int DurationMinutes { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMandatory { get; set; } = true;
    }
}
