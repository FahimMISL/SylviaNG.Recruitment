namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models
{
    public class ExamSeatPlanCreateRequest
    {
        public long ExamId { get; set; }
        public long ExamHallId { get; set; }
        public long ExamCandidateId { get; set; }
        public string? RoomNumber { get; set; }
        public string? SeatNumber { get; set; }
    }
}
