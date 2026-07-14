using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    public class CandidateDocumentResponse
    {
        public long CandidateDocumentId { get; set; }
        public CandidateDocumentTypeEnum DocumentType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Relative static-file URL, same convention as JobPostingAttachmentResponse.DownloadUrl.
        /// </summary>
        public string DownloadUrl { get; set; } = string.Empty;
    }
}
