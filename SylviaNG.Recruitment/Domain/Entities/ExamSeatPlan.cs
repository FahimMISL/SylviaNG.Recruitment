using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ExamSeatPlan : Audit
{
    public long ExamSeatPlanId { get; set; }
    public long ExamId { get; set; }
    public long ExamHallId { get; set; }
    public long ExamCandidateId { get; set; }
    public string? RoomNumber { get; set; }
    public string? SeatNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Exam Exam { get; set; } = null!;
    public ExamHall ExamHall { get; set; } = null!;
    public ExamCandidate ExamCandidate { get; set; } = null!;
}
