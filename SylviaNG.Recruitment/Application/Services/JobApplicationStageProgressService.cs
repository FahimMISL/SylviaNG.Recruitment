using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.PipelineProgress.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationStageProgressService : IJobApplicationStageProgressService
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IHiringPipelineRepository _hiringPipelineRepository;
        private readonly IJobApplicationStageProgressRepository _stageProgressRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        // StageType is free text in the pipeline builder (ManageHiringPipelineComponent's
        // "Suggested" list is just a picker, not a constraint) - so an exam-driven auto-complete
        // asking for the literal "TechnicalAssessment" string would silently no-op if whoever
        // built this pipeline instead typed "OnlineTest" or "CodingTest" from that same suggested
        // list. A pipeline only ever has one exam/test-taking stage in practice, so widening the
        // match to every assessment-flavored suggestion carries no ambiguity risk (unlike, say,
        // widening "TechnicalInterview" across a multi-round interview pipeline, which could
        // complete the wrong round - left untouched on purpose).
        private static readonly string[] TechnicalAssessmentStageTypeAliases =
        {
            "TechnicalAssessment", "OnlineTest", "CodingTest", "WrittenTest", "AptitudeTest", "PsychometricTest", "PracticalAssessment",
        };

        public JobApplicationStageProgressService(
            IJobApplicationRepository jobApplicationRepository,
            IHiringPipelineRepository hiringPipelineRepository,
            IJobApplicationStageProgressRepository stageProgressRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _hiringPipelineRepository = hiringPipelineRepository;
            _stageProgressRepository = stageProgressRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<JobApplicationPipelineProgressResponse> GetByJobApplicationIdAsync(long jobApplicationId)
        {
            var application = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            if (application.JobPosting.HiringPipelineId == null)
            {
                return new JobApplicationPipelineProgressResponse
                {
                    JobApplicationId = jobApplicationId,
                    HasPipeline = false
                };
            }

            var existingProgress = await _stageProgressRepository.GetByJobApplicationIdAsync(jobApplicationId);

            var pipeline = await _hiringPipelineRepository.GetByIdWithStagesAsync(application.JobPosting.HiringPipelineId.Value)
                ?? throw new NotFoundException("HiringPipeline", application.JobPosting.HiringPipelineId.Value);

            if (existingProgress.Count == 0)
            {
                var newProgress = pipeline.Stages
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => s.ToProgressEntity(jobApplicationId))
                    .ToList();

                await _stageProgressRepository.AddRangeAsync(newProgress);
                await _unitOfWork.SaveChangesAsync();

                existingProgress = newProgress;
            }

            var liveStagesById = pipeline.Stages.ToDictionary(s => s.PipelineStageId);

            return new JobApplicationPipelineProgressResponse
            {
                JobApplicationId = jobApplicationId,
                HasPipeline = true,
                PipelineName = pipeline.Name,
                Stages = existingProgress
                    .OrderBy(p => p.DisplayOrder)
                    .Select(p => p.ToResponse(liveStagesById.GetValueOrDefault(p.PipelineStageId)))
                    .ToList()
            };
        }

        public async Task UpdateStageAsync(long jobApplicationId, long pipelineStageId, PipelineStageProgressUpdateRequest request)
        {
            var allProgress = await _stageProgressRepository.GetByJobApplicationIdAsync(jobApplicationId);
            var progress = allProgress.FirstOrDefault(p => p.PipelineStageId == pipelineStageId)
                ?? throw new NotFoundException("JobApplicationStageProgress", pipelineStageId);

            PipelineStage? targetStage = null;
            if (request.Status.HasValue || request.Score.HasValue)
            {
                var owningApplication = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                    a => a.JobApplicationId == jobApplicationId,
                    a => a.JobPosting);

                if (owningApplication?.JobPosting.HiringPipelineId != null)
                {
                    var pipeline = await _hiringPipelineRepository.GetByIdWithStagesAsync(owningApplication.JobPosting.HiringPipelineId.Value);
                    targetStage = pipeline?.Stages.FirstOrDefault(s => s.PipelineStageId == pipelineStageId);

                    // Same mandatory-stage-order gate as BulkAdvanceToStageAsync, now also
                    // enforced on the single-stage manual Update path - previously this was the
                    // only way to move a stage forward, and it had no ordering check at all, so a
                    // mandatory earlier stage could be silently skipped. Only checked when
                    // actually advancing into InProgress/Completed; editing notes/score without
                    // touching status, or moving backward, never trips this (mirrors
                    // EnsureMandatoryStagesCompleted's own doc comment on the bulk path).
                    if (pipeline != null && targetStage != null && request.Status is StageProgressStatusEnum.InProgress or StageProgressStatusEnum.Completed)
                        EnsureMandatoryStagesCompleted(pipeline, allProgress, targetStage);
                }
            }

            // Score has no client-side or server-side bound today - HR can type -50 or 106 into
            // what's meant to be a 0-100 (or 0-MaxMarks, for assessment-shaped stages) rating.
            // MaxMarks null means the stage was never configured as a scored assessment, in which
            // case Score is still conceptually a percentage - default the ceiling to 100.
            if (request.Score.HasValue)
            {
                var maxMarks = targetStage?.MaxMarks ?? 100;
                if (request.Score.Value < 0 || request.Score.Value > maxMarks)
                    throw new FluentValidation.ValidationException(new[]
                    {
                        new FluentValidation.Results.ValidationFailure(nameof(request.Score), $"Score must be between 0 and {maxMarks}.")
                    });
            }

            var wasCompleted = progress.Status == StageProgressStatusEnum.Completed;
            var userName = _currentUserService.GetCurrentUserName();

            progress.ApplyUpdate(request);
            progress.LastUpdatedByUserName = userName;

            _stageProgressRepository.Update(progress);

            // Auto-progression only fires on the transition INTO Completed (not on every
            // re-save while already Completed) - same non-bump semantics as CompletedAt itself
            // in PipelineProgressMapper.ApplyUpdate.
            if (!wasCompleted && progress.Status == StageProgressStatusEnum.Completed && progress.Score.HasValue)
                await TryAutoProgressAsync(jobApplicationId, progress, userName);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BulkAdvanceToStageAsync(List<long> jobApplicationIds, long pipelineStageId)
        {
            var userName = _currentUserService.GetCurrentUserName();

            foreach (var jobApplicationId in jobApplicationIds.Distinct())
                await AdvanceApplicationToStageAsync(jobApplicationId, pipelineStageId, userName);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>Moves one application's progress row into InProgress at the given stage,
        /// provisioning the row first if it doesn't exist yet. Shared by BulkAdvanceToStageAsync
        /// (HR-triggered) and TryAutoProgressAsync (score-triggered) - identical effect either
        /// way, just a different caller deciding when to fire it.</summary>
        private async Task AdvanceApplicationToStageAsync(long jobApplicationId, long pipelineStageId, string? userName)
        {
            // A row provisioned just now by EnsureProgressRowAsync is still tracked as
            // Added (JobApplicationStageProgressId is the identity column's unset 0) -
            // calling Update() on it here would throw ("temporary value ... Modified"),
            // since EF can't move an untracked-in-the-DB row to Modified. Only rows that
            // already existed before this call need the explicit Update() to be marked
            // Modified; a freshly-added row's property changes ride along in its own INSERT.
            var progress = await EnsureProgressRowAsync(jobApplicationId, pipelineStageId);
            var alreadyPersisted = progress.JobApplicationStageProgressId != 0;

            if (progress.Status != StageProgressStatusEnum.InProgress)
                progress.StageEnteredAt = DateTime.UtcNow;

            progress.Status = StageProgressStatusEnum.InProgress;
            progress.LastUpdatedByUserName = userName;

            if (alreadyPersisted)
                _stageProgressRepository.Update(progress);
        }

        /// <summary>PipelineStage.AutoProgressionTargetDisplayOrder + PassMarks, evaluated against
        /// the score just entered on this stage. Both the current stage's config and the target
        /// stage are re-resolved live from the pipeline by DisplayOrder every call - never cached
        /// or stored by PipelineStageId, since stage IDs go stale on every pipeline edit (see
        /// PipelineStage.AutoProgressionTargetDisplayOrder's own doc comment). Silently no-ops if
        /// the pipeline, current stage config, or a qualifying target can't be resolved - a
        /// missing/edited pipeline must never block saving the score itself.</summary>
        private async Task TryAutoProgressAsync(long jobApplicationId, JobApplicationStageProgress progress, string? userName)
        {
            var application = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting);

            if (application?.JobPosting.HiringPipelineId == null)
                return;

            var pipeline = await _hiringPipelineRepository.GetByIdWithStagesAsync(application.JobPosting.HiringPipelineId.Value);
            var currentStage = pipeline?.Stages.FirstOrDefault(s => s.PipelineStageId == progress.PipelineStageId);

            if (currentStage?.PassMarks == null || currentStage.AutoProgressionTargetDisplayOrder == null)
                return;

            if (progress.Score!.Value < currentStage.PassMarks.Value)
                return;

            var targetStage = pipeline!.Stages.FirstOrDefault(s => s.IsActive && s.DisplayOrder == currentStage.AutoProgressionTargetDisplayOrder.Value);
            if (targetStage == null || targetStage.PipelineStageId == currentStage.PipelineStageId)
                return;

            await AdvanceApplicationToStageAsync(jobApplicationId, targetStage.PipelineStageId, userName);
        }

        /// <summary>Returns the progress row for this application at this stage, provisioning
        /// the application's full stage set first if it has none yet (same as
        /// GetByJobApplicationIdAsync), and provisioning just this one row if the application
        /// already has other stages' progress but not this one (e.g. a stage added to the
        /// pipeline after the application's rows were first created).</summary>
        private async Task<JobApplicationStageProgress> EnsureProgressRowAsync(long jobApplicationId, long pipelineStageId)
        {
            var application = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            if (application.JobPosting.HiringPipelineId == null)
                throw new InvalidStatusTransitionException($"JobApplication {jobApplicationId}'s job posting has no hiring pipeline assigned.");

            var pipeline = await _hiringPipelineRepository.GetByIdWithStagesAsync(application.JobPosting.HiringPipelineId.Value)
                ?? throw new NotFoundException("HiringPipeline", application.JobPosting.HiringPipelineId.Value);

            var targetStage = pipeline.Stages.FirstOrDefault(s => s.PipelineStageId == pipelineStageId)
                ?? throw new NotFoundException("PipelineStage", pipelineStageId);

            var existingProgress = await _stageProgressRepository.GetByJobApplicationIdAsync(jobApplicationId);

            if (existingProgress.Count == 0)
            {
                var newProgress = pipeline.Stages
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => s.ToProgressEntity(jobApplicationId))
                    .ToList();

                await _stageProgressRepository.AddRangeAsync(newProgress);
                existingProgress = newProgress;
            }

            // Mandatory stage gate: only checked when advancing FORWARD (skipping a mandatory
            // stage that's still Pending/InProgress is blocked); moving back to an earlier stage
            // never trips this, since nothing with a smaller DisplayOrder than the target exists
            // to check.
            EnsureMandatoryStagesCompleted(pipeline, existingProgress, targetStage);

            var targetProgress = existingProgress.FirstOrDefault(p => p.PipelineStageId == pipelineStageId);
            if (targetProgress != null)
                return targetProgress;

            targetProgress = targetStage.ToProgressEntity(jobApplicationId);
            await _stageProgressRepository.AddAsync(targetProgress);

            return targetProgress;
        }

        public async Task AutoCompleteStageByTypeAsync(long jobApplicationId, string stageType, decimal score, string source)
        {
            var application = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting);
            if (application?.JobPosting.HiringPipelineId == null)
                return;

            var pipeline = await _hiringPipelineRepository.GetByIdWithStagesAsync(application.JobPosting.HiringPipelineId.Value);
            if (pipeline == null)
                return;

            var existingProgress = await _stageProgressRepository.GetByJobApplicationIdAsync(jobApplicationId);
            if (existingProgress.Count == 0)
            {
                var newProgress = pipeline.Stages
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => s.ToProgressEntity(jobApplicationId))
                    .ToList();

                await _stageProgressRepository.AddRangeAsync(newProgress);
                existingProgress = newProgress;
            }

            var matchTypes = string.Equals(stageType, "TechnicalAssessment", StringComparison.OrdinalIgnoreCase)
                ? TechnicalAssessmentStageTypeAliases
                : new[] { stageType };

            var progress = existingProgress
                .Where(p => matchTypes.Any(t => string.Equals(p.StageType, t, StringComparison.OrdinalIgnoreCase)))
                .Where(p => p.Status != StageProgressStatusEnum.Completed)
                .OrderBy(p => p.DisplayOrder)
                .FirstOrDefault();
            if (progress == null)
                return;

            var targetStage = pipeline.Stages.FirstOrDefault(s => s.PipelineStageId == progress.PipelineStageId);
            if (targetStage == null)
                return;

            try
            {
                EnsureMandatoryStagesCompleted(pipeline, existingProgress, targetStage);
            }
            catch (InvalidStatusTransitionException)
            {
                // An earlier mandatory stage isn't Completed yet - an auto-source score can't
                // skip it either, same as the manual Update path. Leave the stage as-is; HR will
                // see it's still blocked on the tracker.
                return;
            }

            var alreadyPersisted = progress.JobApplicationStageProgressId != 0;

            progress.Status = StageProgressStatusEnum.Completed;
            progress.Score = score;
            progress.CompletedAt = DateTime.UtcNow;
            progress.LastUpdatedByUserName = source;

            if (alreadyPersisted)
                _stageProgressRepository.Update(progress);

            await TryAutoProgressAsync(jobApplicationId, progress, source);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EnsureStagePrerequisitesMetAsync(long jobApplicationId, string stageType)
        {
            var application = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting);
            if (application?.JobPosting.HiringPipelineId == null)
                return;

            var pipeline = await _hiringPipelineRepository.GetByIdWithStagesAsync(application.JobPosting.HiringPipelineId.Value);
            var targetStage = pipeline?.Stages.FirstOrDefault(s => string.Equals(s.StageType, stageType, StringComparison.OrdinalIgnoreCase));
            if (pipeline == null || targetStage == null)
                return;

            var existingProgress = await _stageProgressRepository.GetByJobApplicationIdAsync(jobApplicationId);
            EnsureMandatoryStagesCompleted(pipeline, existingProgress, targetStage);
        }

        private static void EnsureMandatoryStagesCompleted(HiringPipeline pipeline, List<JobApplicationStageProgress> existingProgress, PipelineStage targetStage)
        {
            var skippedMandatoryStageNames = pipeline.Stages
                .Where(s => s.IsActive && s.IsMandatory && s.DisplayOrder < targetStage.DisplayOrder)
                .Where(s => existingProgress.FirstOrDefault(p => p.PipelineStageId == s.PipelineStageId)?.Status != StageProgressStatusEnum.Completed)
                .OrderBy(s => s.DisplayOrder)
                .Select(s => s.Name)
                .ToList();

            if (skippedMandatoryStageNames.Count > 0)
            {
                throw new InvalidStatusTransitionException(
                    $"Cannot advance to \"{targetStage.Name}\" - mandatory stage(s) not yet Completed: {string.Join(", ", skippedMandatoryStageNames)}.");
            }
        }
    }
}
