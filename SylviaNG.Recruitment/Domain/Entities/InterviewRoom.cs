using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A physical room within an InterviewVenue, with its own capacity (max concurrent interviews
/// running in that room at overlapping times) - EP-08.
/// </summary>
public class InterviewRoom : Audit
{
    public long InterviewRoomId { get; set; }
    public long InterviewVenueId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public InterviewVenue InterviewVenue { get; set; } = null!;
}
