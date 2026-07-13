using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobPostingService : IJobPostingService
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        // US-016: legal job posting status transitions.
        private static readonly Dictionary<JobStatusEnum, JobStatusEnum[]> LegalStatusTransitions = new()
        {
            [JobStatusEnum.Draft] = new[] { JobStatusEnum.Open },
            [JobStatusEnum.Open] = new[] { JobStatusEnum.OnHold, JobStatusEnum.Closed },
            [JobStatusEnum.OnHold] = new[] { JobStatusEnum.Open },
            [JobStatusEnum.Closed] = new[] { JobStatusEnum.Archived },
            [JobStatusEnum.Archived] = Array.Empty<JobStatusEnum>()
        };

        public JobPostingService(
            IJobPostingRepository jobPostingRepository,
            IUnitOfWork _unitOfWork,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _jobPostingRepository = jobPostingRepository;
            this._unitOfWork = _unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<long> CreateAsync(JobPostingCreateRequest request)
        {
            var exists = await _jobPostingRepository.ExistsByTitleAndSiteIdAsync(request.Title, request.SiteId);
            if (exists)
                throw new DuplicateException("JobPosting", "Title", request.Title);

            var entity = request.ToEntity();
            await _jobPostingRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // JobPostingCode depends on the auto-generated PK, so it can only be
            // computed after the first save; persist it with a second save.
            entity.JobPostingCode = $"JOB-{DateTime.UtcNow:yyyy}-{entity.JobPostingId:D6}";
            _jobPostingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.JobPostingId;
        }

        public async Task UpdateAsync(long jobPostingId, JobPostingUpdateRequest request)
        {
            var entity = await _jobPostingRepository.GetByIdAsync(jobPostingId)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            if (request.Status.HasValue && request.Status.Value != entity.Status)
            {
                EnsureLegalStatusTransition(entity.Status, request.Status.Value);
            }

            entity.ApplyUpdate(request);
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = TryGetCurrentUserId();

            _jobPostingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private static void EnsureLegalStatusTransition(JobStatusEnum currentStatus, JobStatusEnum requestedStatus)
        {
            if (!LegalStatusTransitions.TryGetValue(currentStatus, out var allowedTransitions)
                || !allowedTransitions.Contains(requestedStatus))
            {
                throw new InvalidStatusTransitionException("JobPosting", currentStatus, requestedStatus);
            }
        }

        private long? TryGetCurrentUserId()
        {
            // Best-effort: the current hardcoded-auth JWT does not carry a numeric user-id claim,
            // only username/role, so this will typically remain null.
            var user = _httpContextAccessor?.HttpContext?.User;
            var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
            return long.TryParse(idClaim, out var userId) ? userId : null;
        }

        public async Task DeleteAsync(long jobPostingId)
        {
            var entity = await _jobPostingRepository.GetByIdAsync(jobPostingId)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            _jobPostingRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<JobPostingResponse> GetByIdAsync(long jobPostingId)
        {
            var entity = await _jobPostingRepository.GetByIdWithIncludeAsync(
                j => j.JobPostingId == jobPostingId,
                j => j.Applications)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            return entity.ToResponse();
        }

        public async Task<List<JobPostingResponse>> GetAllAsync()
        {
            var entities = await _jobPostingRepository.GetAllWithIncludeAsync(j => j.Applications);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PagedResult<JobPostingResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _jobPostingRepository.GetPaginatedAsync(request);

            return new PagedResult<JobPostingResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<List<JobPostingLookupResponse>> GetActiveBySiteIdAsync(long siteId)
        {
            var entities = await _jobPostingRepository.GetActiveBySiteIdAsync(siteId);
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }
    }
}
