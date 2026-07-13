using SylviaNG.Recruitment.Application.Features.PipelineProgress.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class PipelineProgressMapper
    {
        public static PipelineStageProgressResponse ToResponse(this JobApplicationStageProgress entity)
        {
            return new PipelineStageProgressResponse
            {
                PipelineStageId = entity.PipelineStageId,
                StageName = entity.StageName,
                StageType = entity.StageType,
                DisplayOrder = entity.DisplayOrder,
                Status = entity.Status,
                ScheduledDate = entity.ScheduledDate,
                MeetingLink = entity.MeetingLink,
                Notes = entity.Notes,
                CompletedAt = entity.CompletedAt,
                LastUpdatedByUserName = entity.LastUpdatedByUserName
            };
        }

        public static void ApplyUpdate(this JobApplicationStageProgress entity, PipelineStageProgressUpdateRequest request)
        {
            if (request.Status.HasValue)
            {
                // Stamp CompletedAt only on the transition into Completed, not on every
                // re-PATCH while already Completed.
                if (request.Status.Value == StageProgressStatusEnum.Completed && entity.Status != StageProgressStatusEnum.Completed)
                    entity.CompletedAt = DateTime.UtcNow;

                entity.Status = request.Status.Value;
            }

            if (request.ScheduledDate.HasValue) entity.ScheduledDate = request.ScheduledDate.Value;
            if (request.MeetingLink != null) entity.MeetingLink = request.MeetingLink;
            if (request.Notes != null) entity.Notes = request.Notes;
        }

        /// <summary>Snapshots a pipeline's stage definition into a fresh Pending progress row (US-042).</summary>
        public static JobApplicationStageProgress ToProgressEntity(this PipelineStage stage, long jobApplicationId)
        {
            return new JobApplicationStageProgress
            {
                JobApplicationId = jobApplicationId,
                PipelineStageId = stage.PipelineStageId,
                StageName = stage.Name,
                StageType = stage.StageType,
                DisplayOrder = stage.DisplayOrder,
                Status = StageProgressStatusEnum.Pending
            };
        }
    }
}
