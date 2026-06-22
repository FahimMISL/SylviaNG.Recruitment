namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models
{
    public class ExamSeatPlanUpdateRequest
    {
        public long? ExamId { get; set; }
        public long? ExamHallId { get; set; }
        public long? ExamCandidateId { get; set; }
        public string? RoomNumber { get; set; }
        public string? SeatNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}
