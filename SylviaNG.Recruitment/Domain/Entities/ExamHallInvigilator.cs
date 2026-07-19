namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Join entity: invigilators (Employees) assigned to an ExamHall.
/// </summary>
public class ExamHallInvigilator
{
    public long ExamHallId { get; set; }
    public long EmployeeId { get; set; }

    // Navigation properties
    public ExamHall ExamHall { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
