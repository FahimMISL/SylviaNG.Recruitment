using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ExportRequestMapper
    {
        public static ExportRequestResponse ToResponse(this ExportRequest entity)
        {
            return new ExportRequestResponse
            {
                ExportRequestId = entity.ExportRequestId,
                ExportType = entity.ExportType,
                Format = entity.Format,
                Status = entity.Status,
                RequestedByUserName = entity.RequestedByUserName,
                RequestedAt = entity.RequestedAt,
                CompletedAt = entity.CompletedAt,
                ExpiresAt = entity.ExpiresAt,
                FileName = entity.FileName,
                RowCount = entity.RowCount,
                FailureReason = entity.FailureReason
            };
        }
    }
}
