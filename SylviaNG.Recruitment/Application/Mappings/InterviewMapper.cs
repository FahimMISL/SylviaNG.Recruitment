using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>Manual mapping methods for Interview. No AutoMapper, matching ExamMapper/ExamEnrollmentMapper.</summary>
    public static class InterviewMapper
    {
        public static Interview ToEntity(this InterviewScheduleRequest request)
        {
            return new Interview
            {
                JobApplicationId = request.JobApplicationId,
                PipelineStageId = request.PipelineStageId,
                InterviewType = request.InterviewType,
                InterviewVenueId = request.InterviewVenueId,
                InterviewRoomId = request.InterviewRoomId,
                MeetingLink = request.MeetingLink,
                ScheduledStartAt = request.ScheduledStartAt,
                ScheduledEndAt = request.ScheduledEndAt,
                Round = request.Round,
                InterviewRoundConfigId = request.InterviewRoundConfigId,
                Notes = request.Notes,
            };
        }

        public static InterviewResponse ToResponse(this Interview entity)
        {
            return new InterviewResponse
            {
                InterviewId = entity.InterviewId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                JobPostingId = entity.JobApplication?.JobPostingId ?? 0,
                PipelineStageId = entity.PipelineStageId,
                InterviewType = entity.InterviewType,
                InterviewVenueId = entity.InterviewVenueId,
                VenueName = entity.InterviewVenue?.VenueName,
                InterviewRoomId = entity.InterviewRoomId,
                RoomName = entity.InterviewRoom?.RoomName,
                MeetingLink = entity.MeetingLink,
                ScheduledStartAt = entity.ScheduledStartAt,
                ScheduledEndAt = entity.ScheduledEndAt,
                Round = entity.Round,
                InterviewRoundConfigId = entity.InterviewRoundConfigId,
                RoundConfigName = entity.InterviewRoundConfig?.Name,
                Result = entity.Result,
                Status = entity.Status,
                CancellationReason = entity.CancellationReason,
                PanelistEmployeeIds = entity.PanelMembers.Select(p => p.EmployeeId).ToList(),
                EmailNotificationStatus = entity.EmailNotificationStatus,
                EmailFailureReason = entity.EmailFailureReason,
                SmsNotificationStatus = entity.SmsNotificationStatus,
                Notes = entity.Notes,
            };
        }
    }
}
