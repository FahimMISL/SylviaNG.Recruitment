using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ExamHall : Audit
{
    public long ExamHallId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? VirtualLink { get; set; }
    public int Capacity { get; set; }
    public int? NumberOfRooms { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ExamSeatPlan> SeatPlans { get; set; } = new List<ExamSeatPlan>();
}
