using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for Exam. No AutoMapper, matching ExamRoomMapper.
    /// </summary>
    public static class ExamMapper
    {
        public static Exam ToEntity(this ExamCreateRequest request)
        {
            return new Exam
            {
                JobPostingId = request.JobPostingId,
                Title = request.Title,
                ScheduledStartAt = request.ScheduledStartAt,
                DurationMinutes = request.DurationMinutes,
                TotalMarks = request.TotalMarks,
                PassMarks = request.PassMarks,
                ExamType = request.ExamType,
                ExamVenueId = request.ExamVenueId,
                QuestionGroupId = request.QuestionGroupId,
                IsActive = true,
            };
        }

        public static ExamResponse ToResponse(this Exam entity)
        {
            return new ExamResponse
            {
                ExamId = entity.ExamId,
                JobPostingId = entity.JobPostingId,
                Title = entity.Title,
                ScheduledStartAt = entity.ScheduledStartAt,
                DurationMinutes = entity.DurationMinutes,
                TotalMarks = entity.TotalMarks,
                PassMarks = entity.PassMarks,
                ExamType = entity.ExamType,
                ExamVenueId = entity.ExamVenueId,
                ExamVenueName = entity.ExamVenue?.VenueName,
                QuestionGroupId = entity.QuestionGroupId,
                QuestionGroupName = entity.QuestionGroup?.Name,
                SeatPlanGeneratedAt = entity.SeatPlanGeneratedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
