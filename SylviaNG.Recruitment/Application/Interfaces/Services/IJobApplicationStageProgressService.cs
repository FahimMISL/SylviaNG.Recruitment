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
    }
}
