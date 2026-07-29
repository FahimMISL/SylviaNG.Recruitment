namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>Join entity: suggested panel members (Employees) for one InterviewRoundConfig
/// (EP-08 US-070). Same shape as InterviewPanelMember, but at the template level rather than
/// the operational-instance level.</summary>
public class InterviewRoundConfigPanelMember
{
    public long InterviewRoundConfigId { get; set; }
    public long EmployeeId { get; set; }

    // Navigation properties
    public InterviewRoundConfig InterviewRoundConfig { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
