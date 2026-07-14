namespace SylviaNG.Recruitment.Domain.Events;

/// <summary>
/// Raised when HR submits an application on a candidate's behalf (US-034 AC3: candidate notified
/// by email). Not dispatched anywhere yet - same unconsumed scaffolding as every other DomainEvent
/// in this codebase; real dispatch is EP-09's job.
/// </summary>
public class JobApplicationSubmittedOnBehalfEvent : DomainEvent
{
    public long JobApplicationId { get; set; }
    public string CandidateEmail { get; set; } = string.Empty;
}
