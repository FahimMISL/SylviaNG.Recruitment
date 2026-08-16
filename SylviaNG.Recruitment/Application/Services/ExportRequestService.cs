using System.Security.Claims;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExportRequestService : IExportRequestService
    {
        /// <summary>US-104: how long a Completed/Failed row survives before ExportRequestWorker's
        /// retention sweep deletes it, downloaded or not.</summary>
        private const int RetentionDays = 7;

        private readonly IExportRequestRepository _exportRequestRepository;
        private readonly IJobApplicationService _jobApplicationService;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ExportRequestService(
            IExportRequestRepository exportRequestRepository,
            IJobApplicationService jobApplicationService,
            IJobApplicationRepository jobApplicationRepository,
            ICurrentUserService currentUserService,
            IUserAccountRepository userAccountRepository,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _exportRequestRepository = exportRequestRepository;
            _jobApplicationService = jobApplicationService;
            _jobApplicationRepository = jobApplicationRepository;
            _currentUserService = currentUserService;
            _userAccountRepository = userAccountRepository;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<long?> TryGetCurrentUserCompanyIdAsync()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            var keycloakUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(keycloakUserId))
                return null;

            var account = await _userAccountRepository.GetByKeycloakUserIdWithRolesAsync(keycloakUserId);
            return account?.CompanyId;
        }

        public async Task<long> RequestCandidateListExportAsync(JobApplicationAttributeFilterRequest filter, ExportFormatEnum format)
        {
            // Throws FluentValidation.ValidationException on a bad filter (e.g. candidate-attribute
            // filters without JobPostingId) - same validation the ATS dashboard already runs.
            var matchedIds = await _jobApplicationService.GetDashboardMatchingIdsAsync(filter);

            var companyId = await TryGetCurrentUserCompanyIdAsync();

            var now = DateTime.UtcNow;
            var entity = new ExportRequest
            {
                ExportType = ExportTypeEnum.CandidateListExport,
                Format = format,
                CompanyId = companyId,
                FilterJson = JsonSerializer.Serialize(filter),
                JobApplicationIdsJson = JsonSerializer.Serialize(matchedIds),
                Status = ExportRequestStatusEnum.Pending,
                RequestedByUserName = _currentUserService.GetCurrentUserName(),
                RequestedByEmail = _currentUserService.GetCurrentUserEmail(),
                RequestedAt = now,
                ExpiresAt = now.AddDays(RetentionDays),
                RowCount = matchedIds.Count
            };

            await _exportRequestRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ExportRequestId;
        }

        public async Task<long> RequestJobApplicationTrackerExportAsync(JobApplicationAttributeFilterRequest filter, ExportFormatEnum format)
        {
            var matchedIds = await _jobApplicationService.GetDashboardMatchingIdsAsync(filter);
            var companyId = await TryGetCurrentUserCompanyIdAsync();

            var now = DateTime.UtcNow;
            var entity = new ExportRequest
            {
                ExportType = ExportTypeEnum.JobApplicationTrackerExport,
                Format = format,
                CompanyId = companyId,
                FilterJson = JsonSerializer.Serialize(filter),
                JobApplicationIdsJson = JsonSerializer.Serialize(matchedIds),
                Status = ExportRequestStatusEnum.Pending,
                RequestedByUserName = _currentUserService.GetCurrentUserName(),
                RequestedByEmail = _currentUserService.GetCurrentUserEmail(),
                RequestedAt = now,
                ExpiresAt = now.AddDays(RetentionDays),
                RowCount = matchedIds.Count
            };

            await _exportRequestRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ExportRequestId;
        }

        public async Task<long> RequestBulkCvZipExportAsync(List<long> jobApplicationIds)
        {
            var distinctIds = jobApplicationIds.Distinct().ToList();
            if (distinctIds.Count == 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(jobApplicationIds), "Select at least one candidate.")
                });
            }

            // Critical fix: jobApplicationIds is caller-supplied. JobApplication is
            // ICompanyScoped, so GetByIdsAsync only ever returns rows the caller's own company
            // can see - any id that doesn't resolve here belongs to another company (or doesn't
            // exist) and must be rejected outright, not silently dropped, so a caller can't probe
            // which ids belong to someone else by comparing request success/failure.
            var ownedApplications = await _jobApplicationRepository.FindAsync(a => distinctIds.Contains(a.JobApplicationId));
            var ownedIds = ownedApplications.Select(a => a.JobApplicationId).ToHashSet();
            var foreignIds = distinctIds.Where(id => !ownedIds.Contains(id)).ToList();
            if (foreignIds.Count > 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(jobApplicationIds),
                        $"One or more selected applications do not belong to your company: {string.Join(", ", foreignIds)}.")
                });
            }

            var companyId = await TryGetCurrentUserCompanyIdAsync();

            var now = DateTime.UtcNow;
            var entity = new ExportRequest
            {
                ExportType = ExportTypeEnum.BulkCvZip,
                Format = ExportFormatEnum.Zip,
                CompanyId = companyId,
                JobApplicationIdsJson = JsonSerializer.Serialize(distinctIds),
                Status = ExportRequestStatusEnum.Pending,
                RequestedByUserName = _currentUserService.GetCurrentUserName(),
                RequestedByEmail = _currentUserService.GetCurrentUserEmail(),
                RequestedAt = now,
                ExpiresAt = now.AddDays(RetentionDays),
                RowCount = distinctIds.Count
            };

            await _exportRequestRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ExportRequestId;
        }

        public async Task<PagedResult<ExportRequestResponse>> GetPagedAsync(ExportRequestFilterRequest filter)
        {
            var paged = await _exportRequestRepository.GetPagedAsync(filter.Page, filter.PageSize, filter.Status);

            return new PagedResult<ExportRequestResponse>
            {
                Data = paged.Data.Select(e => e.ToResponse()).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<ExportRequestFileResponse> GetForDownloadAsync(long exportRequestId)
        {
            var entity = await _exportRequestRepository.GetByIdAsync(exportRequestId)
                ?? throw new NotFoundException("ExportRequest", exportRequestId);

            if (entity.Status != ExportRequestStatusEnum.Completed || entity.ContentObjectKey == null)
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(entity.Status), $"Export is not ready for download (status: {entity.Status}).")
                });

            var stream = await _fileStorageService.OpenReadAsync(entity.ContentObjectKey);
            return new ExportRequestFileResponse(stream, entity.ContentType ?? "application/octet-stream", entity.FileName ?? "export.xlsx");
        }
    }
}
