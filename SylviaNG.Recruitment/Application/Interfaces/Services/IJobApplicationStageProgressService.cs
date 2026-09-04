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

        /// <summary>
        /// Same gate as EnsureStagePrerequisitesMetAsync, but resolves the target stage by
        /// PipelineStageId instead of StageType - for callers that already know exactly which
        /// stage card they're scheduling against (e.g. the per-stage "Schedule Interview" link),
        /// so a pipeline with several same-typed stages (two HrInterview rounds, say) gates on
        /// the right one instead of whichever matches the type string first.
        /// </summary>
        Task EnsureStagePrerequisitesForStageAsync(long jobApplicationId, long pipelineStageId);

        /// <summary>
        /// Same effect as AutoCompleteStageByTypeAsync, but completes the given PipelineStageId
        /// directly instead of searching by StageType - for callers that already know exactly
        /// which stage produced the score (e.g. an Interview row's own PipelineStageId). Silently
        /// no-ops if the stage is already Completed or an earlier mandatory stage isn't done yet,
        /// same non-throwing contract as AutoCompleteStageByTypeAsync.
        /// </summary>
        Task AutoCompleteStageAsync(long jobApplicationId, long pipelineStageId, decimal score, string source);
    }
}
