using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class InterviewSession : Audit
{
    public long InterviewSessionId { get; set; }
    public long? RequisitionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public string? Round { get; set; }
    public InterviewModeEnum Mode { get; set; } = InterviewModeEnum.InPerson;
    public DateTime ScheduledDate { get; set; }
    public int DurationMinutes { get; set; }
    public long? InterviewVenueId { get; set; }
    public string? MeetingLink { get; set; }
    public long? ScorecardId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Requisition? Requisition { get; set; }
    public InterviewVenue? InterviewVenue { get; set; }
    public InterviewScorecard? Scorecard { get; set; }
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
