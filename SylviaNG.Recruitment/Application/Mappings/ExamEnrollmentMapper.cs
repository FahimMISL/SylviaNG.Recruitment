using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for ExamEnrollment. No AutoMapper, matching ExamRoomMapper.
    /// Assumes JobApplication/ExamRoom navigation properties are loaded by the caller.
    /// </summary>
    public static class ExamEnrollmentMapper
    {
        public static ExamEnrollmentResponse ToResponse(this ExamEnrollment entity)
        {
            return new ExamEnrollmentResponse
            {
                ExamEnrollmentId = entity.ExamEnrollmentId,
                ExamId = entity.ExamId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                CandidateEmail = entity.JobApplication?.CandidateEmail,
                CandidatePhone = entity.JobApplication?.CandidatePhone,
                ExamRoomId = entity.ExamRoomId,
                ExamRoomName = entity.ExamRoom?.RoomName,
                SeatNumber = entity.SeatNumber,
                EnrolledAt = entity.EnrolledAt,
                EmailNotificationStatus = entity.EmailNotificationStatus,
                EmailSentAt = entity.EmailSentAt,
                EmailFailureReason = entity.EmailFailureReason,
                SmsNotificationStatus = entity.SmsNotificationStatus,
                SmsLoggedAt = entity.SmsLoggedAt,
                StartedAt = entity.StartedAt,
                SubmittedAt = entity.SubmittedAt,
                AttemptStatus = DeriveAttemptStatus(entity),
                Score = entity.Score,
                IsPassed = entity.IsPassed,
                ScoreSource = entity.ScoreSource,
                ScoredAt = entity.ScoredAt,
                ScoredByUserName = entity.ScoredByUserName,
            };
        }

        /// <summary>Not persisted - derived from StartedAt/SubmittedAt nullability (US-058).</summary>
        private static string DeriveAttemptStatus(ExamEnrollment entity)
        {
            if (entity.SubmittedAt != null) return "Submitted";
            if (entity.StartedAt != null) return "InProgress";
            return "NotStarted";
        }
    }
}
