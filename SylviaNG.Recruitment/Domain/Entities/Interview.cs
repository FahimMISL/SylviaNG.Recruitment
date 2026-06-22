using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class Interview : Audit
{
    public long InterviewId { get; set; }
    public long JobApplicationId { get; set; }
    public long? InterviewSessionId { get; set; }
    public long? InterviewerId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public string? Round { get; set; }
    public string? Feedback { get; set; }
    public string? Result { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
    public InterviewSession? InterviewSession { get; set; }
    public ICollection<InterviewEvaluation> Evaluations { get; set; } = new List<InterviewEvaluation>();
}
