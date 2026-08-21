using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationCoreService : IJobApplicationCoreService
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICandidateApplicationLinkResolver _candidateApplicationLinkResolver;

        public JobApplicationCoreService(
            IJobApplicationRepository jobApplicationRepository,
            IJobPostingRepository jobPostingRepository,
            IUnitOfWork unitOfWork,
            ICandidateApplicationLinkResolver candidateApplicationLinkResolver)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobPostingRepository = jobPostingRepository;
            _unitOfWork = unitOfWork;
            _candidateApplicationLinkResolver = candidateApplicationLinkResolver;
        }

        public async Task<long> CreateAsync(JobApplicationCreateRequest request, long? candidateProfileId = null)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(request.JobPostingId)
                ?? throw new NotFoundException("JobPosting", request.JobPostingId);

            if (!string.IsNullOrEmpty(request.CandidateEmail))
            {
                var exists = await _jobApplicationRepository.GetByEmailAndJobPostingIdAsync(request.CandidateEmail, request.JobPostingId);
                if (exists != null)
                    throw new DuplicateException("JobApplication", "CandidateEmail", request.CandidateEmail);
            }

            var entity = request.ToEntity();
            entity.ApplicationStatus = ApplicationStatusEnum.Applied;
            entity.AppliedDate = DateTime.UtcNow;
            entity.CandidateProfileId = candidateProfileId ?? await _candidateApplicationLinkResolver.ResolveCandidateProfileIdAsync(request.CandidateEmail);

            // Multi-tenant: denormalized from the parent posting - a candidate applying has no
            // company of their own, so this is the only source of truth for which tenant owns
            // this application.
            entity.CompanyId = jobPosting.CompanyId;

            await _jobApplicationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.JobApplicationId;
        }

        public async Task UpdateAsync(long jobApplicationId, JobApplicationUpdateRequest request)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            entity.ApplyUpdate(request);
            _jobApplicationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long jobApplicationId)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            _jobApplicationRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<JobApplicationResponse> GetByIdAsync(long jobApplicationId)
        {
            var entity = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<JobApplicationResponse>> GetPaginatedByJobPostingAsync(long jobPostingId, PagedRequest request)
        {
            var pagedResult = await _jobApplicationRepository.GetPaginatedByJobPostingAsync(jobPostingId, request);

            return new PagedResult<JobApplicationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
