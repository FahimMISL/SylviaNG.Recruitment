namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models
{
    public class InterviewEvaluationUpdateRequest
    {
        public List<InterviewEvaluationScoreRequest> Scores { get; set; } = new();
        public string? OverallComments { get; set; }
    }
}
