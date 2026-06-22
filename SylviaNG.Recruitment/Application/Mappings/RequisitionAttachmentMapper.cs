using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class RequisitionAttachmentMapper
    {
        public static RequisitionAttachment ToEntity(this RequisitionAttachmentCreateRequest request)
        {
            return new RequisitionAttachment
            {
                RequisitionId = request.RequisitionId,
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                ContentType = request.ContentType,
                FileSizeBytes = request.FileSizeBytes,
            };
        }

        public static void ApplyUpdate(this RequisitionAttachment entity, RequisitionAttachmentUpdateRequest request)
        {
            if (request.FileName is not null) entity.FileName = request.FileName;
            if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
            if (request.ContentType is not null) entity.ContentType = request.ContentType;
            if (request.FileSizeBytes.HasValue) entity.FileSizeBytes = request.FileSizeBytes;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RequisitionAttachmentResponse ToResponse(this RequisitionAttachment entity)
        {
            return new RequisitionAttachmentResponse
            {
                RequisitionAttachmentId = entity.RequisitionAttachmentId,
                RequisitionId = entity.RequisitionId,
                FileName = entity.FileName,
                FileUrl = entity.FileUrl,
                ContentType = entity.ContentType,
                FileSizeBytes = entity.FileSizeBytes,
                IsActive = entity.IsActive,
            };
        }
    }
}
