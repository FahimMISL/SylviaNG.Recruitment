namespace SylviaNG.Recruitment.Domain.Events;

public class CandidateEvent : DomainEvent
{
    public long CandidateId { get; set; }
    public string? CandidateEmail { get; set; }

    public CandidateEvent(long candidateId, string action)
    {
        CandidateId = candidateId;
        EntityName = "Candidate";
        Action = action;
    }
}
