using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ExportRequestService(
            IExportRequestRepository exportRequestRepository,
            IJobApplicationService jobApplicationService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _exportRequestRepository = exportRequestRepository;
            _jobApplicationService = jobApplicationService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> RequestCandidateListExportAsync(JobApplicationAttributeFilterRequest filter, ExportFormatEnum format)
        {
            // Throws FluentValidation.ValidationException on a bad filter (e.g. candidate-attribute
            // filters without JobPostingId) - same validation the ATS dashboard already runs.
            var matchedIds = await _jobApplicationService.GetDashboardMatchingIdsAsync(filter);

            var now = DateTime.UtcNow;
            var entity = new ExportRequest
            {
                ExportType = ExportTypeEnum.CandidateListExport,
                Format = format,
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

            if (entity.Status != ExportRequestStatusEnum.Completed || entity.Content == null)
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(entity.Status), $"Export is not ready for download (status: {entity.Status}).")
                });

            return new ExportRequestFileResponse(entity.Content, entity.ContentType ?? "application/octet-stream", entity.FileName ?? "export.xlsx");
        }
    }
}
