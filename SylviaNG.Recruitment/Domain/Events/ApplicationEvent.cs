namespace SylviaNG.Recruitment.Domain.Events;

public class ApplicationEvent : DomainEvent
{
    public long JobApplicationId { get; set; }
    public string? ApplicationStatus { get; set; }

    public ApplicationEvent(long jobApplicationId, string action)
    {
        JobApplicationId = jobApplicationId;
        EntityName = "JobApplication";
        Action = action;
    }
}
