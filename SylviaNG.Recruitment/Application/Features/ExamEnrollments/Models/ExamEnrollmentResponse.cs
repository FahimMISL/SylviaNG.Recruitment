using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models
{
    public class ExamEnrollmentResponse
    {
        public long ExamEnrollmentId { get; set; }
        public long ExamId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? CandidateEmail { get; set; }
        public string? CandidatePhone { get; set; }

        public long? ExamRoomId { get; set; }
        public string? ExamRoomName { get; set; }
        public string? SeatNumber { get; set; }

        public DateTime EnrolledAt { get; set; }

        public NotificationStatusEnum EmailNotificationStatus { get; set; }
        public DateTime? EmailSentAt { get; set; }
        public string? EmailFailureReason { get; set; }

        public NotificationStatusEnum SmsNotificationStatus { get; set; }
        public DateTime? SmsLoggedAt { get; set; }
    }
}
