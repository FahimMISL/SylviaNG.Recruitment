using SylviaNG.Recruitment.Application.Features.PipelineProgress.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJobApplicationStageProgressService
    {
        /// <summary>
        /// The candidate's pipeline tracker for one application (US-042 AC1/AC5). Auto-provisions
        /// Pending rows from the job posting's active pipeline stages on first fetch; returns
        /// HasPipeline=false if the job posting has no pipeline assigned.
        /// </summary>
        Task<JobApplicationPipelineProgressResponse> GetByJobApplicationIdAsync(long jobApplicationId);

        /// <summary>Start/complete/schedule/reschedule/add notes to one stage card (US-042 AC3).</summary>
        Task UpdateStageAsync(long jobApplicationId, long pipelineStageId, PipelineStageProgressUpdateRequest request);

        /// <summary>
        /// Sets the given pipeline stage to InProgress for every listed application, provisioning
        /// that application's full progress-row set first if it doesn't have one yet (same
        /// auto-provision logic as GetByJobApplicationIdAsync). No auto-transition graph, same as
        /// UpdateStageAsync - the caller (e.g. the exam Results page bulk-move action, US-060 AC5)
        /// picks the target stage explicitly since nothing in this codebase links a source
        /// (like an Exam) to a specific PipelineStageId.
        /// </summary>
        Task BulkAdvanceToStageAsync(List<long> jobApplicationIds, long pipelineStageId);

        /// <summary>
        /// Auto-completes this application's earliest non-Completed stage whose StageType matches
        /// (case-insensitive), setting Score and Status=Completed directly - for when an external
        /// system (Exam scoring, Interview Evaluation) already produced the authoritative result,
        /// instead of requiring HR to re-type it manually. Silently no-ops (never throws) if there's
        /// no pipeline, no matching stage, the matching stage is already Completed, or an earlier
        /// mandatory stage isn't Completed yet - callers are score-producing side effects of another
        /// action (submitting an exam, marking an interview result) and must never fail that action.
        /// </summary>
        Task AutoCompleteStageByTypeAsync(long jobApplicationId, string stageType, decimal score, string source);

        /// <summary>
        /// Throws InvalidStatusTransitionException if any mandatory stage before the given
        /// StageType (case-insensitive) isn't Completed yet - for other features that represent
        /// doing work FOR a specific stage (e.g. scheduling an interview for the "Technical
        /// Interview" stage), so they can't be used before that stage's own prerequisites are
        /// met. Mirrors the frontend's blockingPriorStage check, enforced server-side so it can't
        /// be bypassed via direct API calls. No-ops silently if there's no pipeline or no stage of
        /// that type configured.
        /// </summary>
        Task EnsureStagePrerequisitesMetAsync(long jobApplicationId, string stageType);
    }
}
