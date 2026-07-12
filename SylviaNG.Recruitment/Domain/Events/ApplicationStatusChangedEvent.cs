namespace SylviaNG.Recruitment.Domain.Events;

/// <summary>
/// Raised on every application status change (US-036 AC4). Not dispatched anywhere yet -
/// same unconsumed scaffolding as every other DomainEvent in this codebase; real dispatch
/// (candidate notification per event-mapping) is EP-09's job.
/// </summary>
public class ApplicationStatusChangedEvent : DomainEvent
{
    public long JobApplicationId { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = string.Empty;
}
