using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ExportRequestMapper
    {
        public static ExportRequest ToEntity(this ExportRequestCreateRequest request)
        {
            return new ExportRequest
            {
                RequestedByUserId = request.RequestedByUserId,
                ExportType = request.ExportType,
                FilterCriteriaJson = request.FilterCriteriaJson,
                ExportStatus = request.ExportStatus,
                FileUrl = request.FileUrl,
                FileName = request.FileName,
                FileFormat = request.FileFormat,
                FileSizeBytes = request.FileSizeBytes,
                RequestedAt = request.RequestedAt,
                CompletedAt = request.CompletedAt,
                FailureReason = request.FailureReason,
            };
        }

        public static void ApplyUpdate(this ExportRequest entity, ExportRequestUpdateRequest request)
        {
            if (request.RequestedByUserId.HasValue) entity.RequestedByUserId = request.RequestedByUserId.Value;
            if (request.ExportType is not null) entity.ExportType = request.ExportType;
            if (request.FilterCriteriaJson is not null) entity.FilterCriteriaJson = request.FilterCriteriaJson;
            if (request.ExportStatus.HasValue) entity.ExportStatus = request.ExportStatus.Value;
            if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
            if (request.FileName is not null) entity.FileName = request.FileName;
            if (request.FileFormat is not null) entity.FileFormat = request.FileFormat;
            if (request.FileSizeBytes.HasValue) entity.FileSizeBytes = request.FileSizeBytes.Value;
            if (request.RequestedAt.HasValue) entity.RequestedAt = request.RequestedAt.Value;
            if (request.CompletedAt.HasValue) entity.CompletedAt = request.CompletedAt;
            if (request.FailureReason is not null) entity.FailureReason = request.FailureReason;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ExportRequestResponse ToResponse(this ExportRequest entity)
        {
            return new ExportRequestResponse
            {
                ExportRequestId = entity.ExportRequestId,
                RequestedByUserId = entity.RequestedByUserId,
                ExportType = entity.ExportType,
                FilterCriteriaJson = entity.FilterCriteriaJson,
                ExportStatus = entity.ExportStatus,
                FileUrl = entity.FileUrl,
                FileName = entity.FileName,
                FileFormat = entity.FileFormat,
                FileSizeBytes = entity.FileSizeBytes,
                RequestedAt = entity.RequestedAt,
                CompletedAt = entity.CompletedAt,
                FailureReason = entity.FailureReason,
                IsActive = entity.IsActive,
            };
        }
    }
}
