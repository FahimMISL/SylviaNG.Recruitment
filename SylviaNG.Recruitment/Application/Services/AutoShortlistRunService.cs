using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AutoShortlistRunService : IAutoShortlistRunService
    {
        private readonly IAutoShortlistRunRepository _autoShortlistRunRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IShortlistScoringService _scoringService;
        private readonly IJobApplicationService _jobApplicationService;
        private readonly GroqSettings _groqSettings;
        private readonly IUnitOfWork _unitOfWork;

        public AutoShortlistRunService(
            IAutoShortlistRunRepository autoShortlistRunRepository,
            IJobPostingRepository jobPostingRepository,
            IJobApplicationRepository jobApplicationRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IShortlistScoringService scoringService,
            IJobApplicationService jobApplicationService,
            IOptions<GroqSettings> groqSettings,
            IUnitOfWork unitOfWork)
        {
            _autoShortlistRunRepository = autoShortlistRunRepository;
            _jobPostingRepository = jobPostingRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _scoringService = scoringService;
            _jobApplicationService = jobApplicationService;
            _groqSettings = groqSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<AutoShortlistRunResponse> RunAsync(AutoShortlistRunRequest request)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(request.JobPostingId)
                ?? throw new NotFoundException("JobPosting", request.JobPostingId);

            ValidateCutoffScore(request.CutoffScore);

            // Fail-fast: a missing AI provider config is one clear 503, not N per-candidate failures.
            if (_scoringService.ProviderName == "Ai" && string.IsNullOrWhiteSpace(_groqSettings.ApiKey))
                throw new GroqUnavailableException("Groq API key is not configured.");

            var applications = await _jobApplicationRepository.GetAllByJobPostingIdAsync(request.JobPostingId);

            var emails = applications
                .Select(a => a.CandidateEmail)
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct()
                .Cast<string>()
                .ToList();

            var profiles = await _candidateProfileRepository.GetByEmailsAsync(emails);
            var profilesByEmail = profiles.ToDictionary(p => p.Email, p => p, StringComparer.OrdinalIgnoreCase);

            var scored = await ScoreAllAsync(jobPosting, applications, profilesByEmail);

            var run = new AutoShortlistRun
            {
                JobPostingId = request.JobPostingId,
                Provider = _scoringService.ProviderName,
                CutoffScore = request.CutoffScore,
                RunAt = DateTimeUtility.NowUtc(),
                Results = scored.Select(s => new AutoShortlistResult
                {
                    JobApplicationId = s.Application.JobApplicationId,
                    Score = s.Result.Score,
                    Explanation = s.Result.Explanation,
                    MatchedSkills = s.MatchedSkills.Count > 0 ? string.Join(", ", s.MatchedSkills) : null,
                    ExperienceBand = s.ExperienceBand,
                    ScoringFailed = s.Result.Failed,
                    ScoringError = s.Result.ErrorMessage
                }).ToList()
            };

            await _autoShortlistRunRepository.AddAsync(run);
            await _unitOfWork.SaveChangesAsync();

            var candidateNames = applications.ToDictionary(a => a.JobApplicationId, a => a.CandidateName);
            return run.ToResponse(candidateNames);
        }

        public async Task<AutoShortlistRunResponse?> GetLatestAsync(long jobPostingId)
        {
            var run = await _autoShortlistRunRepository.GetLatestByJobPostingIdAsync(jobPostingId);
            if (run == null)
                return null;

            var candidateNames = await BuildCandidateNamesAsync(jobPostingId);
            return run.ToResponse(candidateNames);
        }

        public async Task<AutoShortlistRunResponse> AdjustCutoffAsync(long autoShortlistRunId, int cutoffScore)
        {
            var run = await _autoShortlistRunRepository.GetByIdWithResultsAsync(autoShortlistRunId)
                ?? throw new NotFoundException("AutoShortlistRun", autoShortlistRunId);

            ValidateCutoffScore(cutoffScore);

            run.CutoffScore = cutoffScore;
            _autoShortlistRunRepository.Update(run);
            await _unitOfWork.SaveChangesAsync();

            var candidateNames = await BuildCandidateNamesAsync(run.JobPostingId);
            return run.ToResponse(candidateNames);
        }

        public async Task<AutoShortlistResultResponse> OverrideAsync(long autoShortlistResultId, HrOverrideDecisionEnum? decision)
        {
            var result = await _autoShortlistRunRepository.GetResultByIdAsync(autoShortlistResultId)
                ?? throw new NotFoundException("AutoShortlistResult", autoShortlistResultId);

            result.HrOverrideDecision = decision;
            _autoShortlistRunRepository.UpdateResult(result);
            await _unitOfWork.SaveChangesAsync();

            var application = await _jobApplicationRepository.GetByIdAsync(result.JobApplicationId);
            return result.ToResponse(application?.CandidateName ?? "Unknown", result.AutoShortlistRun.CutoffScore);
        }

        public async Task<AutoShortlistApplyResponse> ApplyAsync(long autoShortlistRunId)
        {
            var run = await _autoShortlistRunRepository.GetByIdWithResultsAsync(autoShortlistRunId)
                ?? throw new NotFoundException("AutoShortlistRun", autoShortlistRunId);

            var includedIds = run.Results
                .Where(r => r.HrOverrideDecision.HasValue
                    ? r.HrOverrideDecision.Value == HrOverrideDecisionEnum.Approved
                    : r.Score.HasValue && r.Score.Value >= run.CutoffScore)
                .Select(r => r.JobApplicationId)
                .ToList();

            var bulkResult = await _jobApplicationService.BulkUpdateStatusAsync(new JobApplicationBulkStatusUpdateRequest
            {
                JobApplicationIds = includedIds,
                ToStatus = ApplicationStatusEnum.Shortlisted
            });

            return new AutoShortlistApplyResponse
            {
                TotalProcessed = run.Results.Count,
                TotalShortlisted = bulkResult.SucceededIds.Count,
                TotalFailed = bulkResult.Failed.Count,
                Failures = bulkResult.Failed
            };
        }

        // ── Scoring orchestration ────────────────────────────────────────

        private sealed record ScoredApplication(JobApplication Application, CandidateScoringResult Result, List<string> MatchedSkills, string? ExperienceBand);

        private async Task<List<ScoredApplication>> ScoreAllAsync(
            JobPosting jobPosting,
            List<JobApplication> applications,
            Dictionary<string, CandidateProfile> profilesByEmail)
        {
            using var semaphore = new SemaphoreSlim(Math.Max(1, _groqSettings.MaxConcurrentRequests));

            var tasks = applications.Select(async application =>
            {
                CandidateProfile? profile = null;
                if (!string.IsNullOrEmpty(application.CandidateEmail))
                    profilesByEmail.TryGetValue(application.CandidateEmail, out profile);

                // Deliberate deviation from ShortlistFilterEvaluationService's precedent (which
                // silently skips unmatched applications): AC1 says "each application", so a
                // missing profile is an explicit failed result, not an omission. BuildFacts(null)
                // can't be used to detect this - it returns a normal-shaped, all-empty
                // CandidateFacts, not a sentinel - so the check happens here, before the scorer
                // is ever called.
                if (profile == null)
                    return new ScoredApplication(application, new CandidateScoringResult(null, null, true, "No candidate profile found; cannot score."), new List<string>(), null);

                var facts = CandidateFactService.BuildFacts(profile);

                // Matched skills / experience band (US-037 AC2) are provider-agnostic - computed
                // once here, not duplicated inside each IShortlistScoringService implementation.
                var matchedSkills = CandidateMatchAnalyzer.GetMatchedSkills(jobPosting, facts);
                var experienceBand = CandidateMatchAnalyzer.GetExperienceBand(facts.TotalExperienceYears);

                await semaphore.WaitAsync();
                try
                {
                    var result = await _scoringService.ScoreAsync(jobPosting, facts);
                    return new ScoredApplication(application, result, matchedSkills, experienceBand);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        private async Task<Dictionary<long, string>> BuildCandidateNamesAsync(long jobPostingId)
        {
            var applications = await _jobApplicationRepository.GetAllByJobPostingIdAsync(jobPostingId);
            return applications.ToDictionary(a => a.JobApplicationId, a => a.CandidateName);
        }

        private static void ValidateCutoffScore(int cutoffScore)
        {
            if (cutoffScore is < 0 or > 100)
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(cutoffScore), "Cutoff score must be between 0 and 100.")
                });
        }
    }
}
