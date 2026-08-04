using SylviaNG.Recruitment.Application.Features.PipelineProgress.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class PipelineProgressMapper
    {
        /// <summary>liveStage is the current PipelineStage config for this row's PipelineStageId
        /// (from the live pipeline, not the progress row itself) - pass null if it's gone missing
        /// (pipeline edited since this row was created); Description/PassingCriteria/
        /// RequiredDocuments/EstimatedDurationMinutes just come back null in that case.</summary>
        public static PipelineStageProgressResponse ToResponse(this JobApplicationStageProgress entity, PipelineStage? liveStage = null)
        {
            return new PipelineStageProgressResponse
            {
                PipelineStageId = entity.PipelineStageId,
                StageName = entity.StageName,
                StageType = entity.StageType,
                DisplayOrder = entity.DisplayOrder,
                // Live-joined like Description/PassingCriteria below - defaults to true (matching
                // PipelineStage.IsMandatory's own default) if the stage no longer exists.
                IsMandatory = liveStage?.IsMandatory ?? true,
                StageDescription = liveStage?.Description,
                PassingCriteria = liveStage?.PassingCriteria,
                RequiredDocuments = liveStage?.RequiredDocuments,
                EstimatedDurationMinutes = liveStage?.EstimatedDurationMinutes,
                MaxMarks = liveStage?.MaxMarks,
                Status = entity.Status,
                ScheduledDate = entity.ScheduledDate,
                MeetingLink = entity.MeetingLink,
                Notes = entity.Notes,
                Score = entity.Score,
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

                // Same non-bump semantics for StageEnteredAt on the transition into InProgress
                // (US-109 "Days in Current Stage").
                if (request.Status.Value == StageProgressStatusEnum.InProgress && entity.Status != StageProgressStatusEnum.InProgress)
                    entity.StageEnteredAt = DateTime.UtcNow;

                entity.Status = request.Status.Value;
            }

            if (request.ScheduledDate.HasValue) entity.ScheduledDate = request.ScheduledDate.Value;
            if (request.MeetingLink != null) entity.MeetingLink = request.MeetingLink;
            if (request.Notes != null) entity.Notes = request.Notes;
            if (request.Score.HasValue) entity.Score = request.Score.Value;
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
                Status = StageProgressStatusEnum.Pending,
                RequiresManualApproval = stage.ManualApprovalRequired,
                SlaDaysSnapshot = stage.SlaDays
            };
        }
    }
}
