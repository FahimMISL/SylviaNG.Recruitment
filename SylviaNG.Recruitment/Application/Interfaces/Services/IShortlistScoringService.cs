using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>0-100 Score with a human-readable Explanation, or Failed with an ErrorMessage.</summary>
    public sealed record CandidateScoringResult(int? Score, string? Explanation, bool Failed, string? ErrorMessage);

    /// <summary>
    /// Scores one candidate against one job posting (US-046, AC1/AC2). Two implementations -
    /// ManualShortlistScoringService and AiShortlistScoringService - swap behind this interface
    /// via the ShortlistScoring:Provider config value; callers never branch on which is active.
    /// </summary>
    public interface IShortlistScoringService
    {
        /// <summary>"Manual" or "Ai" - stamped onto AutoShortlistRun.Provider for HR transparency.</summary>
        string ProviderName { get; }

        Task<CandidateScoringResult> ScoreAsync(JobPosting jobPosting, CandidateFactService.CandidateFacts facts, CancellationToken ct = default);
    }
}
