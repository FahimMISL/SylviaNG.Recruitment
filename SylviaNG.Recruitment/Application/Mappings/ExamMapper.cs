using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ExamMapper
    {
        public static Exam ToEntity(this ExamCreateRequest request)
        {
            return new Exam
            {
                RequisitionId = request.RequisitionId,
                AssessmentStageId = request.AssessmentStageId,
                ExamTitle = request.ExamTitle,
                ExamStatus = request.ExamStatus,
                ScheduledStartTime = request.ScheduledStartTime,
                ScheduledEndTime = request.ScheduledEndTime,
                DurationMinutes = request.DurationMinutes,
                TotalMarks = request.TotalMarks,
                PassMarks = request.PassMarks,
                Instructions = request.Instructions,
            };
        }

        public static void ApplyUpdate(this Exam entity, ExamUpdateRequest request)
        {
            if (request.RequisitionId.HasValue) entity.RequisitionId = request.RequisitionId.Value;
            if (request.AssessmentStageId.HasValue) entity.AssessmentStageId = request.AssessmentStageId.Value;
            if (request.ExamTitle is not null) entity.ExamTitle = request.ExamTitle;
            if (request.ExamStatus.HasValue) entity.ExamStatus = request.ExamStatus.Value;
            if (request.ScheduledStartTime.HasValue) entity.ScheduledStartTime = request.ScheduledStartTime.Value;
            if (request.ScheduledEndTime.HasValue) entity.ScheduledEndTime = request.ScheduledEndTime.Value;
            if (request.DurationMinutes.HasValue) entity.DurationMinutes = request.DurationMinutes.Value;
            if (request.TotalMarks.HasValue) entity.TotalMarks = request.TotalMarks.Value;
            if (request.PassMarks.HasValue) entity.PassMarks = request.PassMarks.Value;
            if (request.Instructions is not null) entity.Instructions = request.Instructions;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ExamResponse ToResponse(this Exam entity)
        {
            return new ExamResponse
            {
                ExamId = entity.ExamId,
                RequisitionId = entity.RequisitionId,
                AssessmentStageId = entity.AssessmentStageId,
                ExamTitle = entity.ExamTitle,
                ExamStatus = entity.ExamStatus,
                ScheduledStartTime = entity.ScheduledStartTime,
                ScheduledEndTime = entity.ScheduledEndTime,
                DurationMinutes = entity.DurationMinutes,
                TotalMarks = entity.TotalMarks,
                PassMarks = entity.PassMarks,
                Instructions = entity.Instructions,
                IsActive = entity.IsActive,
            };
        }
    }
}
