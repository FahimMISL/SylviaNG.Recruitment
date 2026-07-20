using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A physical exam hall/venue with its assigned invigilators (US-062). Not yet linked to an
/// Exam entity (US-055/US-056 aren't built) - this is a standalone registry that future exam
/// scheduling and seat-plan features will reference by ExamHallId.
/// </summary>
public class ExamHall : Audit
{
    public long ExamHallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int TotalCapacity { get; set; }
    public bool NotifyInvigilatorsOnAssign { get; set; } = true;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ExamHallInvigilator> Invigilators { get; set; } = new List<ExamHallInvigilator>();
}
