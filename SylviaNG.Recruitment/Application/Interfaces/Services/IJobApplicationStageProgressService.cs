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
    }
}
