namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Join entity: invigilators (Employees) assigned to an ExamRoom.
/// </summary>
public class ExamRoomInvigilator
{
    public long ExamRoomId { get; set; }
    public long EmployeeId { get; set; }

    // Navigation properties
    public ExamRoom ExamRoom { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
