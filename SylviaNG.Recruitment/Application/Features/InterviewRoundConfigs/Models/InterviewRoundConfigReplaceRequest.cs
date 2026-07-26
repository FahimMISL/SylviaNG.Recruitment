namespace SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models
{
    /// <summary>Whole-list replace payload for one job posting's interview rounds (US-070 AC1/AC4) -
    /// same atomic clear-and-rebuild convention as HiringPipelineUpdateRequest.Stages.</summary>
    public class InterviewRoundConfigReplaceRequest
    {
        public List<InterviewRoundConfigRequest> Rounds { get; set; } = new();
    }
}
