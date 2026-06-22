namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Models
{
    public class ExamCandidateResponse
    {
        public long ExamCandidateId { get; set; }
        public long ExamId { get; set; }
        public long JobApplicationId { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public bool IsPassed { get; set; }
        public bool IsAutoGraded { get; set; }
        public bool IsActive { get; set; }
    }
}
