namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>Join entity: interviewers (Employees) assigned as the panel for one scheduled
/// Interview (EP-08). Same shape as PipelineStageInterviewer, but at the operational-instance
/// level rather than the pipeline-template level.</summary>
public class InterviewPanelMember
{
    public long InterviewId { get; set; }
    public long EmployeeId { get; set; }

    // Navigation properties
    public Interview Interview { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
