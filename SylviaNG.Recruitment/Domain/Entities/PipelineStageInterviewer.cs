namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Join entity: interviewers (Employees) assigned to a PipelineStage as default panelists.
/// </summary>
public class PipelineStageInterviewer
{
    public long PipelineStageId { get; set; }
    public long EmployeeId { get; set; }

    // Navigation properties
    public PipelineStage PipelineStage { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
