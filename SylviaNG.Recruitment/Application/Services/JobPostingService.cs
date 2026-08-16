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
        private readonly IUserAccountRepository _userAccountRepository;
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
            IUserAccountRepository userAccountRepository,
            IUnitOfWork _unitOfWork,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _jobPostingRepository = jobPostingRepository;
            _userAccountRepository = userAccountRepository;
            this._unitOfWork = _unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<long> CreateAsync(JobPostingCreateRequest request)
        {
            var exists = await _jobPostingRepository.ExistsByTitleAsync(request.Title);
            if (exists)
                throw new DuplicateException("JobPosting", "Title", request.Title);

            var entity = request.ToEntity();
            entity.CreatedBy = await TryGetCurrentUserAccountIdAsync();

            // Multi-tenant: stamped from the creating Admin/HR's own company. A SuperAdmin
            // (no CompanyId of their own) creating a posting directly - not the normal flow, but
            // not blocked either - leaves this null, meaning only SuperAdmin can see/manage it
            // until a company claims it; acceptable since job postings are ordinarily created by
            // company-scoped Admin/HR users, never SuperAdmin.
            entity.CompanyId = await TryGetCurrentUserCompanyIdAsync();
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
            entity.UpdatedBy = await TryGetCurrentUserAccountIdAsync();

            _jobPostingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<JobPostingResponse>> GetMyPostingsAsync()
        {
            var userAccountId = await TryGetCurrentUserAccountIdAsync();
            if (userAccountId is null)
                return new List<JobPostingResponse>();

            var entities = await _jobPostingRepository.GetByCreatedByAsync(userAccountId.Value);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        private static void EnsureLegalStatusTransition(JobStatusEnum currentStatus, JobStatusEnum requestedStatus)
        {
            if (!LegalStatusTransitions.TryGetValue(currentStatus, out var allowedTransitions)
                || !allowedTransitions.Contains(requestedStatus))
            {
                throw new InvalidStatusTransitionException("JobPosting", currentStatus, requestedStatus);
            }
        }

        /// <summary>
        /// Resolves the current request's local UserAccountId via the Keycloak subject claim.
        /// Best-effort: returns null for the hardcoded-auth scheme (no matching UserAccount row)
        /// or any Keycloak user who predates EP-15's UserAccount table (invited before this
        /// feature existed) - CreatedBy/UpdatedBy simply stay unset for those, same as today.
        /// </summary>
        private async Task<long?> TryGetCurrentUserAccountIdAsync()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            var keycloakUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(keycloakUserId))
                return null;

            return await _userAccountRepository.GetIdByKeycloakUserIdAsync(keycloakUserId);
        }

        // Multi-tenant: resolves the caller's own CompanyId to stamp onto a newly created
        // JobPosting. Safe to read via the normal (filtered) repository lookup - by this point
        // CompanyScopeMiddleware has already resolved CurrentCompanyId to the caller's own
        // company, so their own UserAccount row is visible under the query filter.
        private async Task<long?> TryGetCurrentUserCompanyIdAsync()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            var keycloakUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(keycloakUserId))
                return null;

            var account = await _userAccountRepository.GetByKeycloakUserIdWithRolesAsync(keycloakUserId);
            return account?.CompanyId;
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
                j => j.Applications, j => j.HiringPipeline!, j => j.Department!)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            return entity.ToResponse();
        }

        public async Task<List<JobPostingResponse>> GetAllAsync()
        {
            var entities = await _jobPostingRepository.GetAllNewestFirstAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PagedResult<JobPostingResponse>> GetPaginatedAsync(PagedRequest request)
        {
            // Job-posting search also supports enum values (status and circular type), so the
            // repository applies the complete title/code/location/enum search expression.
            request.SearchProperties = null;
            var pagedResult = await _jobPostingRepository.GetPaginatedAsync(request);

            return new PagedResult<JobPostingResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        private static readonly CircularTypeEnum[] PublicCircularTypes = { CircularTypeEnum.ExternalOnly, CircularTypeEnum.Both };
        private static readonly CircularTypeEnum[] InternalCircularTypes = { CircularTypeEnum.InternalOnly, CircularTypeEnum.Both };

        public async Task<PagedResult<JobPostingResponse>> GetPaginatedPublicAsync(PagedRequest request, string? location, long? departmentId, EmploymentTypeEnum? employmentType, int? maxExperienceYears)
        {
            return await GetPaginatedByAudienceAsync(request, PublicCircularTypes, location, departmentId, employmentType, maxExperienceYears);
        }

        public async Task<PagedResult<JobPostingResponse>> GetPaginatedInternalAsync(PagedRequest request, string? location, long? departmentId, EmploymentTypeEnum? employmentType, int? maxExperienceYears)
        {
            return await GetPaginatedByAudienceAsync(request, InternalCircularTypes, location, departmentId, employmentType, maxExperienceYears);
        }

        private async Task<PagedResult<JobPostingResponse>> GetPaginatedByAudienceAsync(
            PagedRequest request,
            IReadOnlyCollection<CircularTypeEnum> allowedCircularTypes,
            string? location,
            long? departmentId,
            EmploymentTypeEnum? employmentType,
            int? maxExperienceYears)
        {
            var pagedResult = await _jobPostingRepository.GetPaginatedByCircularTypesAsync(
                request, allowedCircularTypes, location, departmentId, employmentType, maxExperienceYears);

            return new PagedResult<JobPostingResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<JobPostingResponse> GetPublicByIdAsync(long jobPostingId)
        {
            var entity = await _jobPostingRepository.GetOpenByIdAndCircularTypesAsync(jobPostingId, PublicCircularTypes)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            return entity.ToResponse();
        }

        public async Task<JobPostingResponse> GetInternalByIdAsync(long jobPostingId)
        {
            var entity = await _jobPostingRepository.GetOpenByIdAndCircularTypesAsync(jobPostingId, InternalCircularTypes)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            return entity.ToResponse();
        }
    }
}
