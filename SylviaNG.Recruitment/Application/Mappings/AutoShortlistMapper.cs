using SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class AutoShortlistMapper
    {
        public static AutoShortlistResultResponse ToResponse(this AutoShortlistResult entity, string candidateName, int cutoffScore)
        {
            var passed = entity.Score.HasValue && entity.Score.Value >= cutoffScore;

            return new AutoShortlistResultResponse
            {
                AutoShortlistResultId = entity.AutoShortlistResultId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = candidateName,
                Score = entity.Score,
                Explanation = entity.Explanation,
                MatchedSkills = string.IsNullOrWhiteSpace(entity.MatchedSkills)
                    ? new List<string>()
                    : entity.MatchedSkills.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                ExperienceBand = entity.ExperienceBand,
                ScoringFailed = entity.ScoringFailed,
                ScoringError = entity.ScoringError,
                Passed = passed,
                HrOverrideDecision = entity.HrOverrideDecision,
                FinalIncluded = entity.HrOverrideDecision.HasValue
                    ? entity.HrOverrideDecision.Value == Domain.Enums.HrOverrideDecisionEnum.Approved
                    : passed
            };
        }

        public static AutoShortlistRunResponse ToResponse(this AutoShortlistRun entity, Dictionary<long, string> candidateNamesByApplicationId)
        {
            var results = entity.Results
                .Select(r => r.ToResponse(candidateNamesByApplicationId.GetValueOrDefault(r.JobApplicationId, "Unknown"), entity.CutoffScore))
                .ToList();

            return new AutoShortlistRunResponse
            {
                AutoShortlistRunId = entity.AutoShortlistRunId,
                JobPostingId = entity.JobPostingId,
                Provider = entity.Provider,
                CutoffScore = entity.CutoffScore,
                RunAt = entity.RunAt,
                TotalApplications = results.Count,
                TotalScored = results.Count(r => !r.ScoringFailed),
                TotalFailed = results.Count(r => r.ScoringFailed),
                Results = results
            };
        }
    }
}
