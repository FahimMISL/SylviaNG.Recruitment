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

            return new JobApplicationPipelineProgressResponse
            {
                JobApplicationId = jobApplicationId,
                HasPipeline = true,
                PipelineName = pipeline.Name,
                Stages = existingProgress.OrderBy(p => p.DisplayOrder).Select(p => p.ToResponse()).ToList()
            };
        }

        public async Task UpdateStageAsync(long jobApplicationId, long pipelineStageId, PipelineStageProgressUpdateRequest request)
        {
            var progress = (await _stageProgressRepository.GetByJobApplicationIdAsync(jobApplicationId))
                .FirstOrDefault(p => p.PipelineStageId == pipelineStageId)
                ?? throw new NotFoundException("JobApplicationStageProgress", pipelineStageId);

            progress.ApplyUpdate(request);
            progress.LastUpdatedByUserName = _currentUserService.GetCurrentUserName();

            _stageProgressRepository.Update(progress);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BulkAdvanceToStageAsync(List<long> jobApplicationIds, long pipelineStageId)
        {
            var userName = _currentUserService.GetCurrentUserName();

            foreach (var jobApplicationId in jobApplicationIds.Distinct())
            {
                // A row provisioned just now by EnsureProgressRowAsync is still tracked as
                // Added (JobApplicationStageProgressId is the identity column's unset 0) -
                // calling Update() on it here would throw ("temporary value ... Modified"),
                // since EF can't move an untracked-in-the-DB row to Modified. Only rows that
                // already existed before this call need the explicit Update() to be marked
                // Modified; a freshly-added row's property changes ride along in its own INSERT.
                var progress = await EnsureProgressRowAsync(jobApplicationId, pipelineStageId);
                var alreadyPersisted = progress.JobApplicationStageProgressId != 0;

                progress.Status = StageProgressStatusEnum.InProgress;
                progress.LastUpdatedByUserName = userName;

                if (alreadyPersisted)
                    _stageProgressRepository.Update(progress);
            }

            await _unitOfWork.SaveChangesAsync();
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

            var targetProgress = existingProgress.FirstOrDefault(p => p.PipelineStageId == pipelineStageId);
            if (targetProgress != null)
                return targetProgress;

            var targetStage = pipeline.Stages.FirstOrDefault(s => s.PipelineStageId == pipelineStageId)
                ?? throw new NotFoundException("PipelineStage", pipelineStageId);

            targetProgress = targetStage.ToProgressEntity(jobApplicationId);
            await _stageProgressRepository.AddAsync(targetProgress);

            return targetProgress;
        }
    }
}
