using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.PipelineProgress.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
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
    }
}
