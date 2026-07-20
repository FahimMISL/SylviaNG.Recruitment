using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A physical room within an ExamVenue, with its own capacity and assigned invigilators (US-062).
/// Seat-plan layout within a room is a future feature - this is deliberately just name/capacity.
/// </summary>
public class ExamRoom : Audit
{
    public long ExamRoomId { get; set; }
    public long ExamVenueId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool NotifyInvigilatorsOnAssign { get; set; } = true;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ExamVenue ExamVenue { get; set; } = null!;
    public ICollection<ExamRoomInvigilator> Invigilators { get; set; } = new List<ExamRoomInvigilator>();
}
