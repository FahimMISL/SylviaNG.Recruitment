namespace SylviaNG.Recruitment.Domain.Events;

public class RequisitionEvent : DomainEvent
{
    public long RequisitionId { get; set; }
    public string? RequisitionStatus { get; set; }

    public RequisitionEvent(long requisitionId, string action)
    {
        RequisitionId = requisitionId;
        EntityName = "Requisition";
        Action = action;
    }
}
