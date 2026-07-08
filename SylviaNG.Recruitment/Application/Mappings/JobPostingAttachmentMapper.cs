using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for JobPostingAttachment. Follows the same pattern as JobPostingMapper.
    /// </summary>
    public static class JobPostingAttachmentMapper
    {
        public static JobPostingAttachmentResponse ToResponse(this JobPostingAttachment entity)
        {
            return new JobPostingAttachmentResponse
            {
                JobPostingAttachmentId = entity.JobPostingAttachmentId,
                JobPostingId = entity.JobPostingId,
                FileName = entity.FileName,
                ContentType = entity.ContentType,
                FileSizeBytes = entity.FileSizeBytes,
                IsActive = entity.IsActive,
                DownloadUrl = "/" + entity.FilePath.TrimStart('/'),
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
