using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class InterviewSessionMapper
    {
        public static InterviewSession ToEntity(this InterviewSessionCreateRequest request)
        {
            return new InterviewSession
            {
                RequisitionId = request.RequisitionId,
                SessionTitle = request.SessionTitle,
                Round = request.Round,
                Mode = request.Mode,
                ScheduledDate = request.ScheduledDate,
                DurationMinutes = request.DurationMinutes,
                InterviewVenueId = request.InterviewVenueId,
                MeetingLink = request.MeetingLink,
                ScorecardId = request.ScorecardId,
            };
        }

        public static void ApplyUpdate(this InterviewSession entity, InterviewSessionUpdateRequest request)
        {
            if (request.RequisitionId.HasValue) entity.RequisitionId = request.RequisitionId.Value;
            if (request.SessionTitle is not null) entity.SessionTitle = request.SessionTitle;
            if (request.Round is not null) entity.Round = request.Round;
            if (request.Mode.HasValue) entity.Mode = request.Mode.Value;
            if (request.ScheduledDate.HasValue) entity.ScheduledDate = request.ScheduledDate.Value;
            if (request.DurationMinutes.HasValue) entity.DurationMinutes = request.DurationMinutes.Value;
            if (request.InterviewVenueId.HasValue) entity.InterviewVenueId = request.InterviewVenueId.Value;
            if (request.MeetingLink is not null) entity.MeetingLink = request.MeetingLink;
            if (request.ScorecardId.HasValue) entity.ScorecardId = request.ScorecardId.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static InterviewSessionResponse ToResponse(this InterviewSession entity)
        {
            return new InterviewSessionResponse
            {
                InterviewSessionId = entity.InterviewSessionId,
                RequisitionId = entity.RequisitionId,
                SessionTitle = entity.SessionTitle,
                Round = entity.Round,
                Mode = entity.Mode,
                ScheduledDate = entity.ScheduledDate,
                DurationMinutes = entity.DurationMinutes,
                InterviewVenueId = entity.InterviewVenueId,
                MeetingLink = entity.MeetingLink,
                ScorecardId = entity.ScorecardId,
                IsActive = entity.IsActive,
            };
        }
    }
}
