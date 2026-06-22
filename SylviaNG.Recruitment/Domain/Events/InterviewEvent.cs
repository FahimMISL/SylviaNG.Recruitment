namespace SylviaNG.Recruitment.Domain.Events;

public class InterviewEvent : DomainEvent
{
    public long InterviewId { get; set; }
    public long? InterviewSessionId { get; set; }

    public InterviewEvent(long interviewId, string action)
    {
        InterviewId = interviewId;
        EntityName = "Interview";
        Action = action;
    }
}
