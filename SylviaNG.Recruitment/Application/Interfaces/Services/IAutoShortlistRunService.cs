using SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAutoShortlistRunService
    {
        Task<AutoShortlistRunResponse> RunAsync(AutoShortlistRunRequest request);
        Task<AutoShortlistRunResponse?> GetLatestAsync(long jobPostingId);
        Task<AutoShortlistRunResponse> AdjustCutoffAsync(long autoShortlistRunId, int cutoffScore);
        Task<AutoShortlistResultResponse> OverrideAsync(long autoShortlistResultId, HrOverrideDecisionEnum? decision);
        Task<AutoShortlistApplyResponse> ApplyAsync(long autoShortlistRunId);
    }
}
