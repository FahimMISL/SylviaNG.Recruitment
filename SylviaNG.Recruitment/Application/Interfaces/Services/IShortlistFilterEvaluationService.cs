using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IShortlistFilterEvaluationService
    {
        /// <summary>
        /// Evaluates a saved or unsaved filter definition against every application of a job
        /// posting, returning how many would pass (US-043 AC5).
        /// </summary>
        Task<ShortlistFilterPreviewResponse> PreviewAsync(ShortlistFilterPreviewRequest request);

        /// <summary>
        /// Applies a saved filter to all applications of a vacancy in one action, moving passing
        /// candidates to Shortlisted and reporting a processed/shortlisted/failed summary (US-044).
        /// </summary>
        Task<ShortlistFilterApplyResponse> ApplyAsync(ShortlistFilterApplyRequest request);
    }
}
