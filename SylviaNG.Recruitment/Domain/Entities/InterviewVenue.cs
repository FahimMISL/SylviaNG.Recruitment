using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A physical interview venue (location) containing one or more InterviewRooms (EP-08).
/// Deliberately a separate entity from ExamVenue - interview venues and exam venues are FK'd
/// into their own domains, there is no shared/polymorphic Venue table in this codebase.
/// </summary>
public class InterviewVenue : Audit, ICompanyScoped
{
    public long InterviewVenueId { get; set; }

    // Multi-tenant: the Company that owns this venue, stamped at creation.
    public long? CompanyId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<InterviewRoom> Rooms { get; set; } = new List<InterviewRoom>();
}
