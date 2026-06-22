namespace SylviaNG.Recruitment.Domain.Events;

public class OfferEvent : DomainEvent
{
    public long GeneratedDocumentId { get; set; }
    public long CandidateId { get; set; }
    public string? AcceptanceStatus { get; set; }

    public OfferEvent(long generatedDocumentId, long candidateId, string action)
    {
        GeneratedDocumentId = generatedDocumentId;
        CandidateId = candidateId;
        EntityName = "GeneratedDocument";
        Action = action;
    }
}
