using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A physical exam venue (location) containing one or more ExamRooms. Not yet linked to an
/// Exam entity (US-055/US-056 aren't built) - this is a standalone registry that future exam
/// scheduling and seat-plan features will reference by ExamRoomId.
/// </summary>
public class ExamVenue : Audit
{
    public long ExamVenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ExamRoom> Rooms { get; set; } = new List<ExamRoom>();
}
