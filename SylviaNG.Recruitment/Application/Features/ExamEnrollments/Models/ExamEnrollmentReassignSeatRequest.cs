namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models
{
    public class ExamEnrollmentReassignSeatRequest
    {
        public long ExamRoomId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
    }
}
